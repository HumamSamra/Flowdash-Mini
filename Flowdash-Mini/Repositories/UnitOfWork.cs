using Flowdash_Mini.Context;
using Flowdash_Mini.Repositories.AppSettings;
using Flowdash_Mini.Repositories.Members;
using Flowdash_Mini.Repositories.Projects;

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
    }
}
