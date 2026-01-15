using Flowdash_Mini.Context;
using Flowdash_Mini.Repositories.Announcements;
using Flowdash_Mini.Repositories.AppSettings;
using Flowdash_Mini.Repositories.Members;
using Flowdash_Mini.Repositories.Projects;
using Flowdash_Mini.Repositories.TaskBoards;
using Flowdash_Mini.Repositories.Tasks;

namespace Flowdash_Mini.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IAppSettingsRepo AppSettings
            => new AppSettingsRepo(_context);

        public IProjectRepo Projects
            => new ProjectRepo(_context);

        public IMemberRepo Members
            => new MemberRepo(_context);

        public ITaskRepo Tasks
            => new TaskRepo(_context);

        public ITaskBoardRepo TaskBoards
            => new TaskBoardRepo(_context);

        public IAnnouncementRepo Announcements
            => new AnnouncementRepo(_context);
    }
}
