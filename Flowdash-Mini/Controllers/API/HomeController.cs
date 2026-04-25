using AutoMapper;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Extensions;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Models.Projects;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.ViewModels.Invites;
using Flowdash_Mini.ViewModels.Notifications;
using Flowdash_Mini.ViewModels.Projects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Controllers.API
{
    [Route("API/[controller]"), ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HomeController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public HomeController(IUnitOfWork unitOfWork, IMapper mapper,
                              UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetProjects")]
        public ActionResult<List<ProjectVM>> GetProjects()
        {
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

            return _mapper.Map<List<ProjectVM>>(projects);
        }

        [HttpGet("GetInvites")]
        public ActionResult<List<UserInviteVM>> GetInvites()
        {
            var list = _unitOfWork.Invites.GetAll(User.GetUserId())
                .ToList();
            return _mapper.Map<List<UserInviteVM>>(list);
        }

        [HttpPost("AcceptInvite/{id}")]
        public ActionResult AcceptInvite(string id)
        {
            var inv = _unitOfWork.Invites.Get(new Guid(id));
            if (inv == null)
            {
                return NotFound("Invite doesn't exist any more");
            }

            if (User.GetUserId() != inv.UserId)
            {
                return Unauthorized("Unauthorized action detected");
            }

            var project = _unitOfWork.Projects.GetById(inv.ProjectId);
            if (project == null)
            {
                return NotFound("Project was not found");
            }


            var member = new ProjectMember();
            member.MemberId = inv.UserId;
            project.Members.Add(member);
            _unitOfWork.Projects.Update(project);
            _unitOfWork.Invites.Delete(inv.Id);

            var members = _unitOfWork.Members.GetAllByProjectCode(
                project.ProjectCode).ToList();
            var admins = members.Where(e => e.MemberType == MemberType.Admin
                || e.MemberType == MemberType.Owner)
                .ToList();
            foreach (var item in admins)
            {
                _unitOfWork.Notifications.PushNotification(
                    item.MemberId, $"{inv.User.FullName} has joined the project {project.ProjectName} by an invite from {inv.CreatedBy}");
            }

            return Ok("You are now part of the project");
        }

        [HttpPost("RejectInvite/{id}")]
        public ActionResult RejectInvite(string id)
        {
            var inv = _unitOfWork.Invites.Get(new Guid(id));
            if (inv == null)
            {
                return NotFound("Invite doesn't exist any more");
            }

            if (User.GetUserId() != inv.UserId)
            {
                return Unauthorized("Unauthorized action detected");
            }

            var project = _unitOfWork.Projects.GetById(inv.ProjectId);
            if (project == null)
            {
                return NotFound("Project was not found");
            }

            _unitOfWork.Invites.Delete(inv.Id);

            return Ok("Invite rejected successfully");
        }

        [HttpPost("JoinProject")]
        public ActionResult JoinProject(string projCode)
        {
            if (string.IsNullOrWhiteSpace(projCode))
            {
                return BadRequest("Please provide a valid project code");
            }

            var project = _unitOfWork.Projects.GetByCode(projCode);
            if (project == null)
            {
                return NotFound("Project not found");
            }

            if (project.Members.Any(e => e.MemberId == User.GetUserId()))
            {
                return BadRequest("You are already a member in this project");
            }

            var userId = User.GetUserId();
            if (project.State == ProjectState.Personal)
            {
                return BadRequest("Sorry you can't join this project because it's personal");
            }
            else if (project.State == ProjectState.Private)
            {
                if (_unitOfWork.Projects.JoinRequestExists(userId, project.Id))
                {
                    return BadRequest("You have already sent a join request, please wait until you get approved!");
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
                return Ok("Join invitation has been sent successfully");
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

                return Ok("You have successfully joined the project");
            }
        }

        [HttpGet("GetAlerts")]
        public ActionResult<List<NotificationVM>> GetAlerts()
        {
            var notifications = _unitOfWork.Notifications.GetAll(User.GetUserId())
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
            return _mapper.Map<List<NotificationVM>>(notifications);
        }
    }
}
