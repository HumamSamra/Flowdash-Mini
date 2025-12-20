using Flowdash_Mini.Models.AppSettings;

namespace Flowdash_Mini.Repositories.AppSettings
{
    public interface IAppSettingsRepo
    {
        IQueryable<AppSetting> GetAll();
        IQueryable<AppSetting> GetBasic();
        AppSetting? Get(string key);
        void Set(string key, string value);
        void Add(AppSetting item);
        void Update(AppSetting item);
        void Delete(string key);
    }
}
