using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Enums
{
    public enum AppTaskStatus
    {
        [Display(Name = "In Progress")]
        InProgress,

        [Display(Name = "Completed")]
        Completed
    }
}
