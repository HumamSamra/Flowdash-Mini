using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.Projects
{
    public class ProjectAnnouncement
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}
