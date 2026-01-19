using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Announcements
{
    public class CreateAnnouncementVM
    {
        [Required]
        public string Text { get; set; } = string.Empty;
    }
}
