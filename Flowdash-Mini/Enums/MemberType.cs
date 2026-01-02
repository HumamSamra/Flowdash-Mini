using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Enums
{
    public enum MemberType
    {
        [Display(Name = "User")]
        User = 1,

        [Display(Name = "Admin")]
        Admin = 2,

        [Display(Name = "Owner")]
        Owner = 3,
    }
}
