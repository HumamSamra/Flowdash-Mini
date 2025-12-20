using Flowdash_Mini.Repositories.AppSettings;

namespace Flowdash_Mini.Repositories
{
    public interface IUnitOfWork
    {
        IAppSettingsRepo AppSettings { get; }
    }
}
