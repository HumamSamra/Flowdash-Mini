using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Enums
{
    public enum ProjectState
    {
        [Display(Name = "Public")]
        Public = 0,

        [Display(Name = "Private")]
        Private = 1,

        [Display(Name = "Personal")]
        Personal = 2
    }
}
