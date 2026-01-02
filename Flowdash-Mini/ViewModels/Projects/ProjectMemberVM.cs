using Flowdash_Mini.Enums;
using Flowdash_Mini.ViewModels.Accounts;

namespace Flowdash_Mini.ViewModels.Projects
{
    public class ProjectMemberVM
    {
        public AppUserVM Member { get; set; } = null!;
        public ProjectVM Project { get; set; } = null!;
        public MemberType MemberType { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
