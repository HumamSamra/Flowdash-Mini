using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Views
{
    public class ContactVM
    {
        [MaxLength(50)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Message { get; set; } = string.Empty;
    }
}
