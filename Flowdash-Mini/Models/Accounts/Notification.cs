using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.Accounts
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        [Required]
        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}
