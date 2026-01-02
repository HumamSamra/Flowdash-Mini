using Flowdash_Mini.Repositories.AppSettings;
using Flowdash_Mini.Repositories.Members;
using Flowdash_Mini.Repositories.Projects;

namespace Flowdash_Mini.Repositories
{
    public interface IUnitOfWork
    {
        IAppSettingsRepo AppSettings { get; }
        IProjectRepo Projects { get; }
        IMemberRepo Members { get; }
    }
}
