using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Dtos.Accounts
{
    public class UserDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;
    }
}
