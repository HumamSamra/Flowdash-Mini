using Flowdash_Mini.ViewModels.Projects;

namespace Flowdash_Mini.ViewModels.Announcements
{
    public class ProjectAnnouncementVM
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public ProjectVM Project { get; set; } = null!;
        public Guid ProjectId { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}
