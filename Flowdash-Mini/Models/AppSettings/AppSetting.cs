using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Models.AppSettings
{
    public class AppSetting
    {
        public AppSetting(string key, string value,
            bool system = false, bool encrypted = false)
        {
            Key = key;
            Value = value;
            System = system;
            Encrypted = encrypted;
        }

        public AppSetting()
        {

        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Key { get; set; } = null!;

        [Required]
        public string Value { get; set; } = null!;

        // System variables are not deletable
        public bool System { get; set; }
        public bool Encrypted { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        [Required]
        public string ModifiedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
