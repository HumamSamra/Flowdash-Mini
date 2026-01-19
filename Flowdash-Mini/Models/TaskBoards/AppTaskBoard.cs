using Flowdash_Mini.Models.Projects;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.TaskBoards
{
    public class AppTaskBoard
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<AppTask> Tasks { get; set; } = new List<AppTask>();

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
