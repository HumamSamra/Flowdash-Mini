using Flowdash_Mini.Context;
using Flowdash_Mini.Models.AppSettings;

namespace Flowdash_Mini.Repositories.AppSettings
{
    public class AppSettingsRepo : IAppSettingsRepo
    {
        private readonly AppDbContext _context;
        public AppSettingsRepo(AppDbContext context)
        {
            _context = context;
        }

        public void Add(AppSetting item)
        {
            _context.AppSettings.Add(item);
            _context.SaveChanges();
        }

        public void Delete(string key)
        {
            var item = Get(key);
            if (item != null)
            {
                _context.AppSettings.Remove(item);
                _context.SaveChanges();
            }
        }

        public AppSetting? Get(string key)
        {
            return _context.AppSettings.FirstOrDefault(e => e.Key == key);
        }

        public IQueryable<AppSetting> GetAll()
        {
            return _context.AppSettings.AsQueryable();
        }

        public IQueryable<AppSetting> GetBasic()
        {
            return _context.AppSettings
                .Where(e => !e.Key.ToLower().Contains("smtp"))
                .AsQueryable();
        }

        public void Set(string key, string value)
        {
            var item = _context.AppSettings.FirstOrDefault(e => e.Key.ToLower() == key);
            if (item != null)
            {
                item.Value = value;
                _context.AppSettings.Update(item);
                _context.SaveChanges();
            }
        }

        public void Update(AppSetting item)
        {
            var existingItem = Get(item.Key);
            if (existingItem != null)
            {
                existingItem.Value = item.Value;
                existingItem.ModifiedBy = item.ModifiedBy;
                existingItem.ModifiedAt = DateTime.UtcNow;
                _context.AppSettings.Update(existingItem);
                _context.SaveChanges();
            }
        }
    }
}
