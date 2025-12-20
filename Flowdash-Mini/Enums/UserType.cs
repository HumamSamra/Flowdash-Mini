using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Enums
{
    public enum UserType
    {
        [Display(Name = "Migration")]
        Migration = 0,

        [Display(Name = "User")]
        User = 1,

        [Display(Name = "Admin")]
        Admin = 2,
    }
}
