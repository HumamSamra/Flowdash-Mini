using Flowdash_Mini.Models.Projects;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.Accounts
{
    public class AppUser : IdentityUser<Guid>
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public DateTime LastEmailVerificationSent { get; set; }

        public ICollection<ProjectMember> Projects { get; set; } = new List<ProjectMember>();
        public ICollection<ProjectJoinRequest> JoinRequests { get; set; } = new List<ProjectJoinRequest>();
        public ICollection<ProjectInvite> ProjectInvites { get; set; } = new List<ProjectInvite>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
    }
}