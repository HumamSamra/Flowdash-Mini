using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Accounts
{
    public class ApiUserVM
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
