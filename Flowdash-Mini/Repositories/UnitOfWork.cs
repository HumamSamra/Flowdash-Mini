using Flowdash_Mini.Context;
using Flowdash_Mini.Repositories.AppSettings;

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
    }
}
