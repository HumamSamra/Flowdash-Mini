using AutoMapper;
using Flowdash_Mini.Classes;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Extensions;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.Services.CaptchaService;
using Flowdash_Mini.ViewModels.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private const string CookieName = "MEMORY_PROJECT_CODE";
        private readonly ICaptchaService _captcha;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProjectController(IUnitOfWork unitOfWork, IMapper mapper,
                                ICaptchaService captcha)
        {
            _captcha = captcha;
            _mapper = mapper;
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
                RedirectToAction("Index", "Home");
                return;
            }

            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null || !project.Members.Any(e => e.MemberId == User.GetUserId()))
            {
                CookieHandler.Delete(CookieName, HttpContext);
                RedirectToAction("Index", "Home");
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

        public IActionResult Members()
        {
            if (ViewBag.MemberType == MemberType.User)
            {
                return Redirect("/project");
            }

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

            if (project.State == ProjectState.Public ||
                project.State == ProjectState.Private)
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
