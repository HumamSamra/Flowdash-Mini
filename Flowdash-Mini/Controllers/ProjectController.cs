using AutoMapper;
using Flowdash_Mini.Classes;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Extensions;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Models.Projects;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.Services.CaptchaService;
using Flowdash_Mini.ViewModels.Announcements;
using Flowdash_Mini.ViewModels.Members;
using Flowdash_Mini.ViewModels.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private const string CookieName = "MEMORY_PROJECT_CODE";
        private readonly UserManager<AppUser> _userManager;
        private readonly ICaptchaService _captcha;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProjectController(IUnitOfWork unitOfWork, IMapper mapper,
                                ICaptchaService captcha, UserManager<AppUser> userManager)
        {
            _captcha = captcha;
            _mapper = mapper;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        // Ensures project existance and that the user's access
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var code = CookieHandler.Get(CookieName, HttpContext);
            if (string.IsNullOrWhiteSpace(code))
            {
                CookieHandler.Delete(CookieName, HttpContext);
                context.Result = RedirectToAction("Index", "Home");
                return;
            }

            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null || !project.Members.Any(e => e.MemberId == User.GetUserId()))
            {
                CookieHandler.Delete(CookieName, HttpContext);
                context.Result = RedirectToAction("Index", "Home");
                return;
            }

            ViewBag.MemberType = project.Members.First(
                e => e.MemberId == User.GetUserId()).MemberType;
        }

        public IActionResult Index()
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var project = _unitOfWork.Projects.GetByCode(code)!;
            return View(_mapper.Map<ProjectVM>(project));
        }

        public IActionResult Requests()
        {
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Json(new { statusCode = 403, msg = "ACCESS DENIED" });
            }

            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var user = _unitOfWork.Members.GetByUserId(User.GetUserId(), code);
            if (user == null || user.MemberType != MemberType.Admin
                || user.MemberType != MemberType.Owner)
            {
                return Redirect("/project");
            }
            return View();
        }

        public IActionResult Announcements()
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var list = _unitOfWork.Announcements.GetAllByProjectCode(code);

            ViewBag.ProjectCode = code;
            ViewBag.LastAnnouncementId = list.OrderByDescending(
                e => e.CreatedAt).FirstOrDefault()?.Id.ToString() ?? "";

            return View(_mapper.Map<List<ProjectAnnouncementVM>>(
                list.OrderByDescending(e => e.CreatedAt)));
        }

        public JsonResult CreateAnnouncemnet(CreateAnnouncementVM model)
        {
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Json(new { statusCode = 403, msg = "ACCESS DENIED" });
            }

            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project not found" });
            }

            var user = _userManager.Users.FirstOrDefault(
                e => e.Id == User.GetUserId());
            if (user == null)
            {
                return Json(new { statusCode = 404, msg = "User not found" });
            }

            var item = new ProjectAnnouncement()
            {
                CreatedBy = user.FullName,
                ProjectId = project.Id,
                Text = model.Text
            };
            _unitOfWork.Announcements.Create(item);

            return Json(new { statusCode = 200 });
        }

        [HttpPost("/project/deleteannouncement/{id}")]
        public JsonResult DeleteAnnouncement(string id)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            if (ViewBag.MemberType != MemberType.Owner &&
                ViewBag.MemberType != MemberType.Admin)
            {
                return Json(new { statusCode = 403, msg = "ACCESS DENIED" });
            }

            var item = _unitOfWork.Announcements.GetById(new Guid(id));
            if (item == null || item.Project.ProjectCode != code)
            {
                return Json(new { statusCode = 404, msg = "Announcement not found" });
            }

            _unitOfWork.Announcements.Delete(item.Id);
            return Json(new { statusCode = 200 });
        }

        public IActionResult Activity()
        {
            if (ViewBag.MemberType != MemberType.Owner &&
                ViewBag.MemberType != MemberType.Admin)
            {
                return Redirect("/project");
            }

            return View();
        }

        public IActionResult Progress()
        {
            return View();
        }

        public IActionResult TaskBoard()
        {
            return View();
        }

        public IActionResult Members()
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Redirect("/project");
            }

            ViewBag.ProjectState = project.State;

            var projectMembers = _unitOfWork.Members.GetAllByProjectCode(code)
                .Include(e => e.Member)
                .OrderByDescending(e => e.MemberType == MemberType.Owner)
                .ThenByDescending(e => e.MemberType == MemberType.Admin)
                .ToList();

            return View(_mapper.Map<List<ProjectMemberVM>>(projectMembers));
        }

        [HttpGet("/project/editmember/{id}")]
        public IActionResult EditMember(string id)
        {
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Redirect("/project/members");
            }

            var member = _unitOfWork.Members.GetById(new Guid(id));
            if (member == null || member.MemberType == MemberType.Owner)
            {
                return Redirect("/project/members");
            }
            return View(_mapper.Map<EditProjectMemberVM>(member));
        }

        [HttpPost("/project/editmember/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditMember(EditProjectMemberVM model)
        {
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Redirect("/project/members");
            }

            var member = _unitOfWork.Members.GetById(model.Id);
            if (member == null)
            {
                ModelState.AddModelError("", "Requested Member was not found");
                return View(model);
            }

            if (model.MemberType == MemberType.Owner)
            {
                ModelState.AddModelError("", "To transfer project ownership, please refer to the members tab");
                return View(model);
            }

            member.MemberType = model.MemberType;
            _unitOfWork.Members.Update(member);
            return Redirect("/project/members");
        }

        [HttpPost("/project/removemember/{id}")]
        public JsonResult RemoveMember(string id)
        {
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Json(new { statusCode = 403, msg = "ACCESS DENIED" });
            }

            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var item = _unitOfWork.Members.GetById(new Guid(id));
            if (item == null || item.Project.ProjectCode != code!)
            {
                return Json(new { statusCode = 404, msg = "Member not found" });
            }

            _unitOfWork.Members.Delete(item.Id);
            return Json(new { statusCode = 200 });
        }

        [HttpPost("/project/transferownership/{id}")]
        public JsonResult TransferOwnership(string id)
        {
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Json(new { statusCode = 403, msg = "ACCESS DENIED" });
            }

            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var item = _unitOfWork.Members.GetById(new Guid(id));
            if (item == null || item.Project.ProjectCode != code!)
            {
                return Json(new { statusCode = 404, msg = "Member not found" });
            }

            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project not found" });
            }
            var owner = project.Members.FirstOrDefault(
                e => e.MemberType == MemberType.Owner);
            if (owner != null)
            {
                owner.MemberType = MemberType.Admin;
                _unitOfWork.Members.Update(owner);
            }

            item.MemberType = MemberType.Owner;
            _unitOfWork.Members.Update(item);

            return Json(new { statusCode = 200 });
        }

        [HttpGet]
        public IActionResult Manage()
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var project = _unitOfWork.Projects.GetByCode(code)!;

            var members = _unitOfWork.Members.GetAllByProjectCode(code).ToList();
            ViewBag.Members = _mapper.Map<List<ProjectMemberVM>>(members);

            return View(_mapper.Map<EditProjectVM>(project));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(EditProjectVM model)
        {
            var members = _unitOfWork.Members.GetAllByProjectId(model.Id).ToList();
            ViewBag.Members = _mapper.Map<List<ProjectMemberVM>>(members);

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

            var project = _unitOfWork.Projects.GetById(model.Id)!;
            if (project == null)
            {
                ModelState.AddModelError("", "Project not found");
                return View(model);
            }

            project.ProjectName = model.ProjectName;
            project.Description = model.Description;
            project.Tags = model.Tags;

            if (model.State == ProjectState.Personal ||
                model.State == ProjectState.Private)
            {
                if (model.State == ProjectState.Personal &&
                    project.Members.Count > 1)
                {
                    ModelState.AddModelError("", "Cannot change state to Personal when other members exist");
                    return View(model);
                }
            }
            else
            {
                project.State = model.State;
            }

            project.MaxMembersLimit = model.MaxMembersLimit;
            if (project.Members.Count > model.MaxMembersLimit)
            {
                ModelState.AddModelError("", "You cannot set the member limit below the current number of members");
                return View(model);
            }

            project.ModifiedBy = User.GetUserName();
            project.ModifiedAt = DateTime.UtcNow;

            var date = model.DateRange.Split("-");
            project.StartDate = DateTime.Parse(date.First().Trim());
            project.Deadline = DateTime.Parse(date.Last().Trim());

            _unitOfWork.Projects.Update(project);

            return RedirectToAction(nameof(Index));
        }
    }
}
