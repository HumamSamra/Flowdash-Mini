using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.Accounts
{
    public class AppUser : IdentityUser<Guid>
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
