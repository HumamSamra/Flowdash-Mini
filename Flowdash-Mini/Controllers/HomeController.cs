using AutoMapper;
using Flowdash_Mini.Classes;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Extensions;
using Flowdash_Mini.Models;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Models.Projects;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.Services.CaptchaService;
using Flowdash_Mini.ViewModels.Accounts;
using Flowdash_Mini.ViewModels.Notifications;
using Flowdash_Mini.ViewModels.Projects;
using Flowdash_Mini.ViewModels.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Flowdash_Mini.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string CookieName = "MEMORY_PROJECT_CODE";
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICaptchaService _captcha;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork,
                                IMapper mapper, ICaptchaService captcha,
                                UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _captcha = captcha;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var cookie = CookieHandler.Get(CookieName, HttpContext);
            if (string.IsNullOrWhiteSpace(cookie))
            {
                return Redirect("/selectproject");
            }
            else
            {
                var proj = _unitOfWork.Projects.GetByCode(cookie);
                if (proj == null || !proj.Members.Any(e => e.MemberId == User.GetUserId()))
                {
                    CookieHandler.Delete(CookieName, HttpContext);
                    return Redirect("/selectproject");
                }
                return Redirect("/project");
            }
        }

        [HttpGet("/Notifications")]
        public IActionResult Notifications()
        {
            var notifications = _unitOfWork.Notifications.GetAll(User.GetUserId())
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
            var list = _mapper.Map<List<NotificationVM>>(notifications);
            return View(list);
        }

        [HttpGet("/Invites")]
        public IActionResult Invites()
        {
            return View();
        }

        [HttpGet("/security")]
        public IActionResult Security()
        {
            return View();
        }

        [HttpPost("/security")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Security(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User was not found");
                return View(model);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);
            }

            if (model.LogEveryoneOut)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                await _signInManager.SignOutAsync();
                return Redirect("/auth/login?returnUrl=/user/security");
            }

            ViewBag.Success = "Password has been changed successfully";
            return View(model);
        }

        [HttpGet("/MyAccount")]
        public async Task<IActionResult> MyAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/");
            }
            var model = _mapper.Map<UserVM>(user);
            return View(model);
        }

        [HttpPost("/MyAccount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyAccount(UserVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/");
            }

            bool logout = false;
            if (model.Email.Trim().ToLower() != user.Email!.Trim().ToLower())
            {
                logout = true;
                user.EmailConfirmed = false;
            }

            var map = _mapper.Map(model, user);
            map.ModifiedBy = User.GetUserName();
            map.ModifiedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(map);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                    return View(model);
                }
            }

            if (logout)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                await _signInManager.SignOutAsync();
                return Redirect("/auth/login?returnUrl=/myaccount");
            }

            _unitOfWork.Notifications.PushNotification(
                User.GetUserId(), "Your account information has been updated");
            ViewBag.Success = true;

            return View(model);
        }

        [HttpGet("/createproject")]
        public IActionResult CreateProject()
        {
            return View(new CreateProjectVM());
        }

        [HttpPost("/createproject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(CreateProjectVM model)
        {
            var reCaptchaToken = HttpContext.Request.Form["g-recaptcha-response"].ToString();
            if (!await _captcha.VerifyAsync(reCaptchaToken))
            {
                ModelState.AddModelError(string.Empty, "Invalid reCaptcha token");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var project = _mapper.Map<Project>(model);

            while (_unitOfWork.Projects.GetAll().Any(
                    e => e.ProjectCode == project.ProjectCode))
            {
                // If project code already exists, generate a new one
                project.ProjectCode = Tools.GenerateRandomString();
            }

            project.CreatedBy = User.GetUserName();
            project.ModifiedBy = User.GetUserName();

            var date = model.DateRange.Split("-");
            project.StartDate = DateTime.Parse(date.First().Trim());
            project.Deadline = DateTime.Parse(date.Last().Trim());

            // Add user as a new member
            var member = new ProjectMember();
            member.MemberId = User.GetUserId();
            member.MemberType = MemberType.Owner;
            project.Members.Add(member);

            _unitOfWork.Projects.Create(project);
            _unitOfWork.Notifications.PushNotification(
                User.GetUserId(), $"Project '{project.ProjectName}' has been created successfully");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("/joinproject")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> JoinProject(string projCode)
        {
            var reCaptchaToken = HttpContext.Request.Form["g-recaptcha-response"].ToString();
            if (!await _captcha.VerifyAsync(reCaptchaToken))
            {
                return Json(new { statusCode = 400, msg = "Invalid reCaptcha token" });
            }

            if (string.IsNullOrWhiteSpace(projCode))
            {
                return Json(new { statusCode = 400, msg = "Please provide a valid project code" });
            }

            var project = _unitOfWork.Projects.GetByCode(projCode);
            if (project == null)
            {
                return Json(new { statusCode = 400, msg = "Project not found" });
            }

            if (project.Members.Any(e => e.MemberId == User.GetUserId()))
            {
                return Json(new { statusCode = 400, msg = "You are already a member in this project" });
            }

            var userId = User.GetUserId();
            if (project.State == ProjectState.Personal)
            {
                return Json(new { statusCode = 400, msg = "Sorry you can't join this project because it's personal" });
            }
            else if (project.State == ProjectState.Private)
            {
                if (_unitOfWork.Projects.JoinRequestExists(userId, project.Id))
                {
                    return Json(new
                    {
                        statusCode = 400,
                        msg = "You have already sent a join request, please wait until you get approved!"
                    });
                }

                var request = new ProjectJoinRequest(userId, project.Id);
                _unitOfWork.Projects.CreateProjectJoinRequest(request);

                _unitOfWork.Notifications.PushNotification(
                    User.GetUserId(), $"Join request has been sent to project '{project.ProjectName}' successfully");

                var user = _userManager.Users.FirstOrDefault(e => e.Id == userId);
                var owner = project.Members.FirstOrDefault(e => e.MemberType == MemberType.Owner);
                if (owner != null && user != null)
                {
                    _unitOfWork.Notifications.PushNotification(
                        owner.MemberId, $"{user.FullName} has requested to join the project '{project.ProjectName}'");
                }
                return Json(new { statusCode = 200, msg = "Join invitation has been sent successfully" });
            }
            else
            {
                var member = new ProjectMember();
                member.MemberId = User.GetUserId();
                project.Members.Add(member);
                _unitOfWork.Projects.Update(project);

                _unitOfWork.Notifications.PushNotification(
                    User.GetUserId(), $"You are now a member of '{project.ProjectName}'");

                var owner = project.Members.FirstOrDefault(e => e.MemberType == MemberType.Owner);
                if (owner != null)
                {
                    _unitOfWork.Notifications.PushNotification(
                        owner.MemberId, $"{member.Member.FullName} has joined the project '{project.ProjectName}'");
                }

                return Json(new { statusCode = 200 });
            }
        }

        [HttpGet("/SelectProject")]
        public IActionResult SelectProject()
        {
            CookieHandler.Delete(CookieName, HttpContext);

            var projects = _unitOfWork.Projects.GetAll()
                .Include(e => e.Members)
                .ThenInclude(e => e.Member)
                .Where(e => e.Members.Any(e => e.MemberId == User.GetUserId()))
                .ToList();

            // Prioritize projects where the current user is an Owner
            projects.OrderByDescending(e => e.Members.First(
                e => e.MemberId == User.GetUserId()).MemberType == MemberType.Owner)
                .ThenByDescending(e => e.Members.First(
                    e => e.MemberId == User.GetUserId()).MemberType == MemberType.Admin);

            return View(_mapper.Map<List<ProjectVM>>(projects));
        }

        [HttpGet("/SetProject/{id}")]
        public IActionResult SetProject(string id, string returnUrl = "/project")
        {
            var project = _unitOfWork.Projects.GetById(new Guid(id));
            if (project == null || !project.Members.Any(
                e => e.MemberId == User.GetUserId()))
            {
                return NotFound();
            }

            CookieHandler.Set(CookieName, HttpContext, project.ProjectCode);

            return Redirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
