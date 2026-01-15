using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.TaskBoards
{
    public class AppTask
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public AppTaskBoard TaskBoard { get; set; } = null!;
        public Guid TaskBoardId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = null!;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; } = null!;
    }
}
