using Flowdash_Mini.Enums;
using Flowdash_Mini.ViewModels.Accounts;
using Flowdash_Mini.ViewModels.Projects;

namespace Flowdash_Mini.ViewModels.Members
{
    public class ProjectMemberVM
    {
        public Guid Id { get; set; }
        public AppUserVM Member { get; set; } = null!;
        public ProjectVM Project { get; set; } = null!;
        public MemberType MemberType { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
