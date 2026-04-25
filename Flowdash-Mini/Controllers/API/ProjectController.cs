using AutoMapper;
using Flowdash_Mini.Dtos.Tasks;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Extensions;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Models.Projects;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.ViewModels.Announcements;
using Flowdash_Mini.ViewModels.Members;
using Flowdash_Mini.ViewModels.TaskBoards;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Controllers.API
{
    [Route("API/[controller]"), ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProjectController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProjectController(IUnitOfWork unitOfWork, IMapper mapper,
                              UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAnnouncements/{projectId}")]
        public ActionResult<List<ProjectAnnouncementVM>> GetAnnouncements(string projectId)
        {
            var list = _unitOfWork.Announcements.GetAll()
                .Where(e => e.ProjectId == new Guid(projectId))
                .ToList();

            return _mapper.Map<List<ProjectAnnouncementVM>>(
                list.OrderByDescending(e => e.CreatedAt));
        }

        [HttpGet("GetMembers/{projectId}")]
        public ActionResult<List<ProjectMemberVM>> GetMembers(string projectId)
        {
            var project = _unitOfWork.Projects.GetById(new Guid(projectId));
            if (project == null)
            {
                return NotFound("Project not found");
            }

            var projectMembers = _unitOfWork.Members.GetAllByProjectCode(project.ProjectCode)
                .Include(e => e.Member)
                .OrderByDescending(e => e.MemberType == MemberType.Owner)
                .ThenByDescending(e => e.MemberType == MemberType.Admin)
                .ToList();

            return _mapper.Map<List<ProjectMemberVM>>(projectMembers);
        }

        [HttpGet("GetTaskboards")]
        public ActionResult<List<TaskBoardVM>> GetTaskBoards(string projectId)
        {
            var taskBoards = _unitOfWork.TaskBoards.GetAll()
                            .Include(e => e.Tasks)
                            .Where(e => e.ProjectId == new Guid(projectId))
                            .ToList();
            return _mapper.Map<List<TaskBoardVM>>(taskBoards);
        }

        [HttpGet("GetTaskboard")]
        public ActionResult<TaskBoardVM> GetTaskBoard(string taskboardId)
        {
            var taskBoard = _unitOfWork.TaskBoards.GetById(new Guid(taskboardId));
            if (taskBoard == null)
            {
                return NotFound("Taskboard not found");
            }
            return _mapper.Map<TaskBoardVM>(taskBoard);
        }

        [HttpPost("SaveSort")]
        public ActionResult SaveSort([FromBody] List<string> sortId)
        {
            for (int i = 1; i <= sortId.Count; i++)
            {
                var task = _unitOfWork.Tasks.GetById(new Guid(sortId[i - 1]));
                if (task != null)
                {
                    task.Sort = i;
                    _unitOfWork.Tasks.Update(task);
                }
            }
            return Ok("Sort order updated successfully");
        }

        [HttpPost("SetTaskStatus")]
        public ActionResult SetTaskStatus(UpdateTaskDto dto)
        {
            var item = _unitOfWork.Tasks.GetById(dto.Id);
            if (item == null)
            {
                return BadRequest("Task was not found");
            }

            var project = item.TaskBoard.Project;
            var member = _unitOfWork.Members.GetByUserId(User.GetUserId(), project.ProjectCode);
            if (member == null)
            {
                return NotFound("User was not found");
            }

            item.Status = dto.Status;
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

            return Ok("Status updated successfully");
        }
    }
}
