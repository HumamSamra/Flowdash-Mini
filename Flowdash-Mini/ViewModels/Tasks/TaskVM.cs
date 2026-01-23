using Flowdash_Mini.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Tasks
{
    public class TaskVM
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public AppTaskStatus Status { get; set; } = AppTaskStatus.InProgress;

        public DateTime CompletedAt { get; set; }
        public string CompletedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = null!;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; } = null!;
    }
}
