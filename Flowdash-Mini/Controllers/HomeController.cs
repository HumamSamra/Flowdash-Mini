using AutoMapper;
using Flowdash_Mini.Classes;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Extensions;
using Flowdash_Mini.Models;
using Flowdash_Mini.Models.Projects;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.Services.CaptchaService;
using Flowdash_Mini.ViewModels.Projects;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ICaptchaService _captcha;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork,
                                IMapper mapper, ICaptchaService captcha)
        {
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
                return Redirect("/project");
            }
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
                ModelState.AddModelError("", "Invalid reCaptcha token");
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

            if (project.State == ProjectState.Personal)
            {
                return Json(new { statusCode = 400, msg = "Sorry you can't join this project because it's personal" });
            }
            else if (project.State == ProjectState.Private)
            {
                return Json(new { statusCode = 200, msg = "Join invitation has been sent successfully" });
            }
            else
            {
                var member = new ProjectMember();
                member.MemberId = User.GetUserId();
                project.Members.Add(member);
                _unitOfWork.Projects.Update(project);
                return Json(new { statusCode = 200 });
            }
        }

        [HttpGet("/SelectProject")]
        public IActionResult SelectProject()
        {
            CookieHandler.Delete(CookieName, HttpContext);

            var projects = _unitOfWork.Projects.GetAll()
                .Include(e => e.Members)
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
