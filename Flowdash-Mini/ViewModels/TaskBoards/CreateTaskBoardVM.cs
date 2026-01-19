using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.TaskBoards
{
    public class CreateTaskBoardVM
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
