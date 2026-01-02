using Flowdash_Mini.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.Projects
{
    public class EditProjectVM
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string ProjectName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public string Tags { get; set; } = string.Empty;

        [ValidateNever]
        public string ProjectCode { get; set; } = string.Empty;

        [Range(1, 99)]
        public int MaxMembersLimit { get; set; } = 20;

        public string DateRange { get; set; } = $"{DateTime.UtcNow} - {DateTime.UtcNow.AddMonths(1)}";

        public ProjectState State { get; set; } = ProjectState.Personal;

        public DateTime CreatedAt { get; set; }
    }
}
