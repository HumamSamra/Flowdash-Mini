using Flowdash_Mini.Context;
using Flowdash_Mini.Models.Projects;

namespace Flowdash_Mini.Repositories.ProjectLogs
{
    public class ProjectLogRepo : IProjectLogRepo
    {
        private readonly AppDbContext _context;
        public ProjectLogRepo(AppDbContext context)
        {
            _context = context;
        }

        public void Create(ProjectLog item)
        {
            item.CreatedAt = DateTime.UtcNow;

            _context.ProjectLogs.Add(item);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _context.ProjectLogs.Remove(item);
                _context.SaveChanges();
            }
        }

        public IQueryable<ProjectLog> GetAll()
            => _context.ProjectLogs.AsQueryable();

        public ProjectLog? GetById(Guid id)
            => _context.ProjectLogs
            .FirstOrDefault(p => p.Id == id);
    }
}
