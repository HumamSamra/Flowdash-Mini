using Flowdash_Mini.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Accounts
{
    public class AppUserVM
    {
        public Guid Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }
        public bool IsLockedout { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        [Required]
        public string ModifiedBy { get; set; } = string.Empty;

        public IList<string> Roles { get; set; } = new List<string> {
            nameof(UserType.User)
        };
    }
}
