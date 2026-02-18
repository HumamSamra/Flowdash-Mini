using Flowdash_Mini.ViewModels.Accounts;
using Flowdash_Mini.ViewModels.Projects;
using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.ViewModels.JoinRequests
{
    public class JoinRequestVM
    {
        [Key]
        public Guid Id { get; set; }

        public UserVM User { get; set; } = null!;
        public ProjectVM Project { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
