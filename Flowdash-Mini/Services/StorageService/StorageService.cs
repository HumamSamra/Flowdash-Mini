using Flowdash_Mini.Context;
using Flowdash_Mini.Models.Files;

namespace Flowdash_Mini.Services.StorageService
{
    public class StorageService : IStorageService
    {
        private readonly static string RootPath = "./wwwroot/Storage/Files";
        private readonly AppDbContext _context;
        public StorageService(AppDbContext context)
        {
            _context = context;
        }

        public DbFile? Get(Guid id)
            => _context.DbFiles.FirstOrDefault(e => e.Id == id);

        public DbFile? GetMain(Guid parentId)
            => _context.DbFiles.FirstOrDefault(
                e => e.ItemId == parentId && e.IsMain);

        public IQueryable<DbFile> GetAll(Guid fileId)
        {
            return _context.DbFiles
                .Where(e => e.ItemId == fileId)
                .OrderByDescending(e => e.CreatedAt)
                .AsQueryable();
        }

        public void Add(DbFile file, IFormFile formFile, bool keepFileName = false)
        {
            try
            {
                file.CreatedAt = DateTime.UtcNow;
                file.ModifiedAt = DateTime.UtcNow;

                file.FileName = keepFileName ?
                    SanitizeFileName(formFile) : UniqueFileName(formFile.FileName);
                file.FileType = formFile.ContentType;

                if (!Directory.Exists(file.DirectoryPath))
                {
                    Directory.CreateDirectory(file.DirectoryPath);
                }

                if (!Directory.Exists($"{RootPath}/{file.Table}/{file.ItemId}"))
                {
                    Directory.CreateDirectory($"{RootPath}/{file.Table}/{file.ItemId}");
                }

                // Store Internally
                if (!File.Exists(file.Path))
                {
                    WriteBytes(formFile, $"{RootPath}/{file.Table}/{file.ItemId}/{file.FileName}");
                }

                _context.Add(file);
                _context.SaveChanges();
            }
            catch (Exception err)
            {
                throw new Exception("Error adding file", err);
            }
        }

        public void Clear(Guid parentId)
        {
            try
            {
                var files = _context.DbFiles
                    .Where(e => e.ItemId == parentId)
                    .ToList();
                if (files.Count == 0)
                {
                    if (Directory.Exists($"{RootPath}/{files[0].Table}/{files[0].ItemId}"))
                    {
                        Directory.Delete($"{RootPath}/{files[0].Table}/{files[0].ItemId}", true);

                        _context.DbFiles.RemoveRange(files);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception("Error clearing files", err);
            }
        }

        public void Delete(Guid id)
        {
            try
            {
                var file = _context.DbFiles.FirstOrDefault(e => e.Id == id);
                if (file != null)
                {
                    if (File.Exists(file.Path))
                    {
                        File.Delete(file.Path);
                        _context.DbFiles.Remove(file);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception("Error deleting file", err);
            }
        }
        public void Update(DbFile file, IFormFile formFile, bool keepFileName = false)
        {
            try
            {
                var existingFile = _context.DbFiles.FirstOrDefault(e => e.Id == file.Id);
                if (existingFile != null)
                {
                    if (File.Exists(existingFile.Path))
                    {
                        File.Delete(existingFile.Path);
                    }

                    if (!Directory.Exists($"{RootPath}/{file.Table}/{file.ItemId}"))
                    {
                        Directory.CreateDirectory($"{RootPath}/{file.Table}/{file.ItemId}");
                    }

                    existingFile.FileName = keepFileName ?
                        SanitizeFileName(formFile) : UniqueFileName(formFile.FileName);
                    existingFile.ItemId = file.ItemId;
                    existingFile.IsMain = file.IsMain;
                    existingFile.FileType = formFile.ContentType;
                    existingFile.ModifiedAt = DateTime.UtcNow;
                    existingFile.ModifiedBy = file.ModifiedBy;

                    // Store Internally
                    WriteBytes(formFile, $"{RootPath}/{file.Table}/{file.ItemId}/{file.FileName}");

                    _context.Update(existingFile);
                }
                _context.SaveChanges();
            }
            catch (Exception err)
            {
                throw new Exception("Error updating file", err);
            }
        }

        private static void WriteBytes(IFormFile file, string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            file.CopyTo(stream);
        }

        public static string SanitizeFileName(IFormFile file)
        {
            string fileName = Path.GetFileName(file.FileName);

            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }
            fileName = fileName.Replace(' ', '_');

            return fileName;
        }

        private static string UniqueFileName(string path)
            => Guid.NewGuid() + Path.GetExtension(path);
    }
}
