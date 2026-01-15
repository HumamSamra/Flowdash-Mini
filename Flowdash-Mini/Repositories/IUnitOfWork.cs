using Flowdash_Mini.Repositories.Announcements;
using Flowdash_Mini.Repositories.AppSettings;
using Flowdash_Mini.Repositories.Members;
using Flowdash_Mini.Repositories.Projects;
using Flowdash_Mini.Repositories.TaskBoards;
using Flowdash_Mini.Repositories.Tasks;

namespace Flowdash_Mini.Repositories
{
    public interface IUnitOfWork
    {
        IAppSettingsRepo AppSettings { get; }
        ITaskRepo Tasks { get; }
        ITaskBoardRepo TaskBoards { get; }
        IProjectRepo Projects { get; }
        IAnnouncementRepo Announcements { get; }
        IMemberRepo Members { get; }
    }
}
