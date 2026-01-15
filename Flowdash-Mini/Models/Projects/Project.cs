using Flowdash_Mini.Classes;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Models.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.Projects
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string ProjectName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(8)]
        public string ProjectCode { get; set; } = Tools.GenerateRandomString();
        public string Tags { get; set; } = string.Empty;

        [Range(1, 99)]
        public int MaxMembersLimit { get; set; } = 20;

        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }

        public ProjectState State { get; set; } = ProjectState.Personal;

        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
        public ICollection<ProjectJoinRequest> JoinRequests { get; set; } = new List<ProjectJoinRequest>();
        public ICollection<ProjectInvite> ProjectInvites { get; set; } = new List<ProjectInvite>();
        public ICollection<ProjectLog> ProjectLogs { get; set; } = new List<ProjectLog>();

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
