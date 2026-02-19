using Flowdash_Mini.Repositories.Announcements;
using Flowdash_Mini.Repositories.AppSettings;
using Flowdash_Mini.Repositories.Invites;
using Flowdash_Mini.Repositories.JoinRequests;
using Flowdash_Mini.Repositories.Members;
using Flowdash_Mini.Repositories.Notifications;
using Flowdash_Mini.Repositories.Projects;
using Flowdash_Mini.Repositories.TaskBoards;
using Flowdash_Mini.Repositories.Tasks;

namespace Flowdash_Mini.Repositories
{
    public interface IUnitOfWork
    {
        IAppSettingsRepo AppSettings { get; }
        IInviteRepo Invites { get; }
        ITaskRepo Tasks { get; }
        ITaskBoardRepo TaskBoards { get; }
        IProjectRepo Projects { get; }
        IJoinRequestRepo JoinRequests { get; }
        IAnnouncementRepo Announcements { get; }
        INotificationRepo Notifications { get; }
        IMemberRepo Members { get; }
    }
}
