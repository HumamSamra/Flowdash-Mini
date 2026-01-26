using Flowdash_Mini.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Tasks
{
    public class CreateTaskVM
    {
        public Guid TaskBoardId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public AppTaskStatus Status { get; set; } = AppTaskStatus.InProgress;
    }
}
