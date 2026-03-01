using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Accounts
{
    public class CredentialsVM
    {
        [Required]
        public string Account { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
