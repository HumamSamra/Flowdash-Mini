using Flowdash_Mini.Enums;

namespace Flowdash_Mini.ViewModels.Members
{
    public class EditProjectMemberVM
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public MemberType MemberType { get; set; }
    }
}
