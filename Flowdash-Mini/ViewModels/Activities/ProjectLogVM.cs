using Flowdash_Mini.Enums;

namespace Flowdash_Mini.ViewModels.Activities
{
    public class ProjectLogVM
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public MemberType CreatedByRole { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
