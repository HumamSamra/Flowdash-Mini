using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flowdash_Mini.Models.Files
{
    public class DbFile
    {
        public DbFile()
        {
        }

        public DbFile(string table, Guid itemId, string createdBy, bool isMain = true)
        {
            Table = table;
            ItemId = itemId;
            IsMain = isMain;
            CreatedBy = createdBy;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string Table { get; set; } = string.Empty;

        [Required]
        public Guid ItemId { get; set; }

        public bool IsMain { get; set; }
        public string FileType { get; set; } = "UnKnown";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;

        [NotMapped]
        public string Path
        {
            get
            {
                return $"/Storage/Files/{Table}/{ItemId}/{FileName}";
            }
        }

        [NotMapped]
        public string DirectoryPath
        {
            get
            {
                return $"/Storage/Files/{Table}/{ItemId}";
            }
        }
    }
}
