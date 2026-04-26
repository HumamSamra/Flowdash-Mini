using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Enums
{
    public enum AppTaskStatus
    {
        [Display(Name = "In Progress")]
        InProgress = 0,

        [Display(Name = "Completed")]
        Completed = 1
    }
}
