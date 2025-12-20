using Flowdash_Mini.Models.Files;

namespace Flowdash_Mini.Services.StorageService
{
    public interface IStorageService
    {
        DbFile? GetMain(Guid parentId);
        DbFile? Get(Guid id);
        IQueryable<DbFile> GetAll(Guid fileId);

        void Add(DbFile file, IFormFile formFile, bool keepFileName = false);
        void Update(DbFile file, IFormFile formFile, bool keepFileName = false);

        void Clear(Guid parentId);
        void Delete(Guid id);
    }
}
