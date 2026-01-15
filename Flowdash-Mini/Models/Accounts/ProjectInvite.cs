using Flowdash_Mini.Models.Projects;

namespace Flowdash_Mini.Models.Accounts
{
    public class ProjectInvite
    {
        public Guid Id { get; set; }
        public bool IsValid { get; set; } = true;

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;
    }
}
