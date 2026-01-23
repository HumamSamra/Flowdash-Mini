using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Accounts
{
    public class UserVM
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
