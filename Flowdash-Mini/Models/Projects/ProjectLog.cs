namespace Flowdash_Mini.Models.Projects
{
    public class ProjectLog
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
