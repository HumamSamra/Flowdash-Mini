using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Accounts
{
    public class ResetPasswordVM
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
