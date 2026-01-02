using Flowdash_Mini.Enums;
using Flowdash_Mini.Models.Accounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flowdash_Mini.Models.Projects
{
    public class ProjectMember
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Member")]
        public Guid MemberId { get; set; }
        public AppUser Member { get; set; } = null!;

        [ForeignKey("Project")]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public MemberType MemberType { get; set; } = MemberType.User;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
