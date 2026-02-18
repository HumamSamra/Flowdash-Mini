using Flowdash_Mini.Models.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.Projects
{
    public class ProjectJoinRequest
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ProjectJoinRequest()
        {

        }

        public ProjectJoinRequest(Guid userId, Guid projId)
        {
            UserId = userId;
            ProjectId = projId;
        }
    }
}
