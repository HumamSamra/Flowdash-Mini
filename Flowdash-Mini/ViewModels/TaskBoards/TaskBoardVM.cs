using Flowdash_Mini.ViewModels.Tasks;

namespace Flowdash_Mini.ViewModels.TaskBoards
{
    public class TaskBoardVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<TaskVM> Tasks { get; set; } = new List<TaskVM>();

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
