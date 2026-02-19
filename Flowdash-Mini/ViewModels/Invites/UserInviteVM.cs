using Flowdash_Mini.ViewModels.Accounts;
using Flowdash_Mini.ViewModels.Projects;

namespace Flowdash_Mini.ViewModels.Invites
{
    public class UserInviteVM
    {
        public Guid Id { get; set; }
        public UserVM User { get; set; } = null!;
        public ProjectVM Project { get; set; } = null!;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
