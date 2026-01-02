using Flowdash_Mini.Context;
using Flowdash_Mini.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Repositories.Projects
{
    public class ProjectRepo : IProjectRepo
    {
        private readonly AppDbContext _context;
        public ProjectRepo(AppDbContext context)
        {
            _context = context;
        }

        public void Create(Project item)
        {
            item.CreatedAt = DateTime.UtcNow;
            item.ModifiedAt = DateTime.UtcNow;

            _context.Projects.Add(item);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _context.Projects.Remove(item);
                _context.SaveChanges();
            }
        }

        public IQueryable<Project> GetAll()
            => _context.Projects.AsQueryable();

        public Project? GetById(Guid id)
            => _context.Projects
            .Include(e => e.Members)
            .FirstOrDefault(p => p.Id == id);

        public Project? GetByCode(string code)
            => _context.Projects
            .Include(e => e.Members)
            .FirstOrDefault(p => p.ProjectCode == code);

        public void Update(Project modifiedItem)
        {
            var item = GetById(modifiedItem.Id);
            if (item != null)
            {
                item.ModifiedAt = DateTime.UtcNow;
                _context.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}
