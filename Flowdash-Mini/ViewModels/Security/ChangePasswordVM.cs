using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Security
{
    public class ChangePasswordVM
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Password and Confirm Password don't match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool LogEveryoneOut { get; set; }
    }
}
