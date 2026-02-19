using Flowdash_Mini.Models.Projects;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.Accounts
{
    public class UserInvite
    {
        [Key]
        public Guid Id { get; set; }

        public AppUser User { get; set; } = null!;
        public Guid UserId { get; set; }

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
