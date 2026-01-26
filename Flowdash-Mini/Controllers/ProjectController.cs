using AutoMapper;
using Flowdash_Mini.Classes;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Extensions;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Models.Projects;
using Flowdash_Mini.Models.TaskBoards;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.Services.CaptchaService;
using Flowdash_Mini.ViewModels.Activities;
using Flowdash_Mini.ViewModels.Announcements;
using Flowdash_Mini.ViewModels.Members;
using Flowdash_Mini.ViewModels.Projects;
using Flowdash_Mini.ViewModels.TaskBoards;
using Flowdash_Mini.ViewModels.Tasks;
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

        [HttpGet]
        public IActionResult Index()
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var project = _unitOfWork.Projects.GetByCode(code)!;

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), code);
            if (member != null && (member.MemberType == MemberType.Owner ||
                member.MemberType == MemberType.Admin))
            {
                var logs = _unitOfWork.Projects.GetLogs(project.Id)
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(4)
                    .ToList();
                ViewBag.LastActivities = _mapper.Map<List<ProjectLogVM>>(logs);
            }

            var tasks = _unitOfWork.Tasks.GetAll()
                .Where(e => e.TaskBoard.ProjectId == project.Id);
            ViewBag.FinishedTasks = tasks.Where(e => e.Status == AppTaskStatus.Completed).Count();
            ViewBag.InProgressTasks = tasks.Where(e => e.Status == AppTaskStatus.InProgress).Count();

            return View(_mapper.Map<ProjectVM>(project));
        }

        [HttpGet]
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

        [HttpGet]
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

        [HttpPost]
        public JsonResult CreateAnnouncemnet(CreateAnnouncementVM model)
        {
            // Restrict Creating Announcements for owners only
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

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), project.Id);
            if (member == null)
            {
                return Json(new { statusCode = 404, msg = "User not found" });
            }

            var item = new ProjectAnnouncement()
            {
                CreatedBy = member.Member.FullName,
                ProjectId = project.Id,
                Text = model.Text
            };
            _unitOfWork.Announcements.Create(item);
            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                member.MemberType, $"Announcement '{model.Text}' was posted successfully"));

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

            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project not found" });
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), project.Id);
            if (member == null)
            {
                return Json(new { statusCode = 404, msg = "User not found" });
            }

            var item = _unitOfWork.Announcements.GetById(new Guid(id));
            if (item == null || item.Project.ProjectCode != code)
            {
                return Json(new { statusCode = 404, msg = "Announcement not found" });
            }

            _unitOfWork.Announcements.Delete(item.Id);
            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                member.MemberType, $"Announcement '{item.Text}' was deleted successfully"));

            return Json(new { statusCode = 200 });
        }

        public IActionResult Activity()
        {
            if (ViewBag.MemberType != MemberType.Owner &&
                ViewBag.MemberType != MemberType.Admin)
            {
                return Redirect("/project");
            }

            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var logs = _unitOfWork.Projects.GetLogs(code)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();

            return View(_mapper.Map<List<ProjectLogVM>>(logs));
        }

        [HttpGet]
        public IActionResult Progress()
        {
            var taskBoards = _unitOfWork.TaskBoards.GetAll()
                .Include(e => e.Tasks)
                .ToList();
            return View(_mapper.Map<List<TaskBoardVM>>(taskBoards));
        }

        [HttpGet("/project/taskboard/{id}")]
        public IActionResult TaskBoard(string id)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var taskBoard = _unitOfWork.TaskBoards.GetById(new Guid(id));
            if (taskBoard == null || taskBoard.Project.ProjectCode != code)
            {
                return Redirect("/project");
            }

            ViewBag.TaskBoardId = taskBoard.Id;
            ViewBag.TaskBoardName = taskBoard.Title;

            var t = _unitOfWork.Tasks.GetAll()
                .Where(e => e.TaskBoardId == taskBoard.Id)
                .ToList();
            return View(_mapper.Map<List<TaskVM>>(t));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateTask(CreateTaskVM model)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return Json(new { statusCode = 400, msg = errors[0] });
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), code);
            if (member == null)
            {
                return Json(new { statusCode = 404, msg = "User was not found" });
            }

            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project not found" });
            }

            var taskBoard = _unitOfWork.TaskBoards.GetById(model.TaskBoardId);
            if (taskBoard == null)
            {
                return Json(new { statusCode = 404, msg = "Task Board not found" });
            }

            var item = _mapper.Map<AppTask>(model);
            item.CreatedBy = member.Member.FullName;
            item.ModifiedBy = member.Member.FullName;

            if (item.Status == AppTaskStatus.Completed)
            {
                item.CompletedAt = DateTime.UtcNow;
                item.CompletedBy = member.Member.FullName;
            }

            item.Sort = 1;
            var maxTaskSort = _unitOfWork.Tasks.GetAll()
                .Where(e => e.TaskBoardId == model.TaskBoardId)
                .ToList();
            if (maxTaskSort.Count > 0)
            {
                item.Sort = maxTaskSort.Max(e => e.Sort) + 1;
            }

            _unitOfWork.Tasks.Create(item);
            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                member.MemberType, $"Task created in Task Board '{taskBoard.Title}' successfully"));

            return Json(new { statusCode = 200 });
        }

        [HttpGet("/project/gettask/{id}")]
        public JsonResult GetTask(string id)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { statusCode = 400, msg = "Please provide a valid Id" });
            }

            var task = _unitOfWork.Tasks.GetById(new Guid(id));
            if (task == null || task.TaskBoard.Project.ProjectCode != code)
            {
                return Json(new { statusCode = 404, msg = "Task was not found" });
            }

            return Json(new { statusCode = 200, item = _mapper.Map<EditTaskVM>(task) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveSort(List<string> sortId)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            for (int i = 1; i <= sortId.Count; i++)
            {
                var task = _unitOfWork.Tasks.GetById(new Guid(sortId[i - 1]));
                if (task != null && task.TaskBoard.Project.ProjectCode == code)
                {
                    task.Sort = i;
                    _unitOfWork.Tasks.Update(task);
                }
            }
            return Json(new { statusCode = 200 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetTaskStatus(string id, AppTaskStatus status)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { statusCode = 400, msg = "Please provide a valid Id" });
            }

            var item = _unitOfWork.Tasks.GetById(new Guid(id));
            if (item == null || item.TaskBoard.Project.ProjectCode != code)
            {
                return Json(new { statusCode = 404, msg = "Task was not found" });
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), code);
            if (member == null)
            {
                return Json(new { statusCode = 404, msg = "User was not found" });
            }

            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project was not found" });
            }

            item.Status = status;
            item.ModifiedAt = DateTime.UtcNow;
            item.ModifiedBy = member.Member.FullName;

            if (item.Status == AppTaskStatus.Completed)
            {
                item.CompletedAt = DateTime.UtcNow;
                item.CompletedBy = member.Member.FullName;
            }

            _unitOfWork.Tasks.Update(item);
            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                member.MemberType, $"Status has been changed for the task '{item.Title}' in Task Board {item.TaskBoard.Title}"));

            return Json(new { statusCode = 200 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditTask(EditTaskVM model)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return Json(new { statusCode = 400, msg = errors[0] });
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), code);
            if (member == null)
            {
                return Json(new { statsuCode = 404, msg = "User was not found" });
            }

            var task = _unitOfWork.Tasks.GetById(model.Id);
            if (task == null || task.TaskBoard.Project.ProjectCode != code)
            {
                return Json(new { statusCode = 404, msg = "Task was not found" });
            }

            var map = _mapper.Map(model, task);
            _unitOfWork.Tasks.Update(map);
            _unitOfWork.Projects.Log(new ProjectLog(member.ProjectId, member.Member.FullName,
                member.MemberType, $"Task '{task.Title}' in '{task.TaskBoard.Title}' was modified successfully"));

            return Json(new { statusCode = 200 });
        }

        [HttpPost("/project/deletetask/{id}")]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteTask(string id)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { statusCode = 400, msg = "Please provide a valid Id" });
            }

            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project was not found" });
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), code);
            if (member == null)
            {
                return Json(new { statusCode = 404, msg = "User not found" });
            }

            var task = _unitOfWork.Tasks.GetById(new Guid(id));
            if (task == null || task.TaskBoard.Project.ProjectCode != code)
            {
                return Json(new { statusCode = 404, msg = "Task was not found" });
            }

            _unitOfWork.Tasks.Delete(task.Id);
            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                member.MemberType, $"Task '{task.Title}' in '{task.TaskBoard.Title}' was deleted successfully"));

            return Json(new { statusCode = 200 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateTaskBoard(CreateTaskBoardVM model)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project not found" });
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), project.Id);
            if (member == null)
            {
                return Json(new { statusCode = 404, msg = "User not found" });
            }

            var taskBoard = _mapper.Map<AppTaskBoard>(model);
            taskBoard.ProjectId = project.Id;
            taskBoard.CreatedBy = member.Member.FullName;
            _unitOfWork.TaskBoards.Create(taskBoard);

            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                member.MemberType, $"Task Board '{taskBoard.Title}' was created successfully"));

            return Json(new { statusCode = 200, });
        }

        [HttpGet]
        public async Task<JsonResult> GetTaskBoard(string id)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project not found" });
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { statusCode = 400, msg = "Please provide a valid Id" });
            }

            var taskBoard = _unitOfWork.TaskBoards.GetById(new Guid(id));
            if (taskBoard == null || taskBoard.Project.ProjectCode != code)
            {
                return Json(new { statusCode = 400, msg = "Task Board was not found" });
            }

            return Json(new { statusCode = 200, data = _mapper.Map<TaskBoardVM>(taskBoard) });
        }

        [HttpPost("/project/deletetaskboard/{id}")]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteTaskBoard(string id)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project not found" });
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { statusCode = 400, msg = "Please provide a valid Id" });
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), project.Id);
            if (member == null)
            {
                return Json(new { statusCode = 404, msg = "User not found" });
            }

            var taskBoard = _unitOfWork.TaskBoards.GetById(new Guid(id));
            if (taskBoard == null || taskBoard.Project.ProjectCode != code)
            {
                return Json(new { statusCode = 400, msg = "Task Board was not found" });
            }

            _unitOfWork.TaskBoards.Delete(taskBoard.Id);

            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                member.MemberType, $"Task Board '{taskBoard.Title}' was deleted successfully"));

            return Json(new { statusCode = 200 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditTaskBoard(EditTaskBoardVM model)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project not found" });
            }

            var taskBoard = _unitOfWork.TaskBoards.GetById(model.Id);
            if (taskBoard == null || taskBoard.Project.ProjectCode != code)
            {
                return Json(new { statusCode = 400, msg = "Task board was not found" });
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), project.Id);
            if (member == null)
            {
                return Json(new { statusCode = 404, msg = "User not found" });
            }

            taskBoard.Title = model.Title ?? "";
            taskBoard.Description = model.Description ?? "";
            taskBoard.ModifiedBy = User.GetUserName();

            _unitOfWork.TaskBoards.Update(taskBoard);
            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                member.MemberType, $"Task Board '{taskBoard.Title}' was modified successfully"));

            return Json(new { statusCode = 200, });
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
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Redirect("/project/members");
            }

            var item = _unitOfWork.Members.GetById(new Guid(id));
            if (item == null || item.MemberType == MemberType.Owner
                || item.Project.ProjectCode != code)
            {
                return Redirect("/project/members");
            }

            return View(_mapper.Map<EditProjectMemberVM>(item));
        }

        [HttpPost("/project/editmember/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditMember(EditProjectMemberVM model)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Redirect("/project/members");
            }

            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Redirect("/project");
            }

            var item = _unitOfWork.Members.GetById(model.Id);
            if (item == null || item.Project.ProjectCode != code)
            {
                ModelState.AddModelError(string.Empty, "Requested Member was not found");
                return View(model);
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), project.Id);
            if (member == null)
            {
                return Json(new { statusCode = 404, msg = "User not found" });
            }

            if (model.MemberType == MemberType.Owner)
            {
                ModelState.AddModelError(string.Empty, "To transfer project ownership, please refer to the members tab");
                return View(model);
            }

            item.MemberType = model.MemberType;
            _unitOfWork.Members.Update(item);

            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                member.MemberType, $"Member '{item.Member.FullName}' was modified successfully"));

            return Redirect("/project/members");
        }

        [HttpPost("/project/removemember/{id}")]
        public JsonResult RemoveMember(string id)
        {
            var code = CookieHandler.Get(CookieName, HttpContext)!;
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Json(new { statusCode = 403, msg = "ACCESS DENIED" });
            }

            var project = _unitOfWork.Projects.GetByCode(code);
            if (project == null)
            {
                return Json(new { statusCode = 404, msg = "Project not found" });
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), project.Id);
            if (member == null || member.Project.ProjectCode != code)
            {
                return Json(new { statusCode = 404, msg = "User not found" });
            }

            var item = _unitOfWork.Members.GetById(new Guid(id));
            if (item == null || item.Project.ProjectCode != code!)
            {
                return Json(new { statusCode = 404, msg = "Member not found" });
            }

            _unitOfWork.Members.Delete(item.Id);
            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                member.MemberType, $"Member '{item.Member.FullName}' was modified successfully"));

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

            _unitOfWork.Projects.Log(new ProjectLog(project.Id, owner.Member.FullName,
                owner.MemberType, $"Ownership transferred from '{owner.Member.FullName}' to '{item.Member.FullName}'"));

            return Json(new { statusCode = 200 });
        }

        [HttpGet]
        public IActionResult Manage()
        {
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Redirect("/project/members");
            }

            var code = CookieHandler.Get(CookieName, HttpContext)!;
            var project = _unitOfWork.Projects.GetByCode(code)!;

            var members = _unitOfWork.Members.GetAllByProjectCode(code).ToList();
            ViewBag.Members = _mapper.Map<List<ProjectMemberVM>>(members);

            return View(_mapper.Map<EditProjectVM>(project));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Manage(EditProjectVM model)
        {
            if (ViewBag.MemberType != MemberType.Owner)
            {
                return Redirect("/project/members");
            }

            var members = _unitOfWork.Members.GetAllByProjectId(model.Id).ToList();
            ViewBag.Members = _mapper.Map<List<ProjectMemberVM>>(members);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var project = _unitOfWork.Projects.GetById(model.Id)!;
            if (project == null)
            {
                ModelState.AddModelError(string.Empty, "Project not found");
                return View(model);
            }

            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), project.Id);
            if (member == null)
            {
                return Json(new { statusCode = 404, msg = "User not found" });
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
                    ModelState.AddModelError(string.Empty, "Cannot change state to Personal when other members exist");
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
                ModelState.AddModelError(string.Empty, "You cannot set the member limit below the current number of members");
                return View(model);
            }



            project.ModifiedBy = User.GetUserName();
            project.ModifiedAt = DateTime.UtcNow;

            var date = model.DateRange.Split("-");
            project.StartDate = DateTime.Parse(date.First().Trim());
            project.Deadline = DateTime.Parse(date.Last().Trim());

            _unitOfWork.Projects.Update(project);

            _unitOfWork.Projects.Log(new ProjectLog(project.Id, member.Member.FullName,
                 member.MemberType, $"Project was modified successfully"));

            return RedirectToAction(nameof(Index));
        }
    }
}
