using Flowdash_Mini.Models.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.Notifications
{
    public class Notification
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Text { get; set; } = string.Empty;

        public AppUser User { get; set; } = null!;
        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
