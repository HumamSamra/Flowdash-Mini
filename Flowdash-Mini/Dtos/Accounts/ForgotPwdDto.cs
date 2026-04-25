using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Dtos.Accounts
{
    public class ForgotPwdDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
