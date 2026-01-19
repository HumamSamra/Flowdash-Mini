using Flowdash_Mini.Enums;

namespace Flowdash_Mini.Models.Projects
{
    public class ProjectLog
    {
        public ProjectLog()
        {

        }
        public ProjectLog(Guid projId, string by, MemberType role, string txt)
        {
            ProjectId = projId;
            Text = txt;
            CreatedBy = by;
            CreatedByRole = role;
        }

        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public MemberType CreatedByRole { get; set; }

        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
