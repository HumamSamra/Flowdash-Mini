using Flowdash_Mini.Context;
using Flowdash_Mini.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Repositories.Announcements
{
    public class AnnouncementRepo : IAnnouncementRepo
    {
        private readonly AppDbContext _context;
        public AnnouncementRepo(AppDbContext context)
        {
            _context = context;
        }

        public void Create(ProjectAnnouncement item)
        {
            item.CreatedAt = DateTime.UtcNow;

            _context.ProjectAnnouncements.Add(item);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _context.ProjectAnnouncements.Remove(item);
                _context.SaveChanges();
            }
        }

        public IQueryable<ProjectAnnouncement> GetAll()
            => _context.ProjectAnnouncements.AsQueryable();

        public List<ProjectAnnouncement> GetAllByProjectCode(string code)
            => _context.ProjectAnnouncements
            .Include(e => e.Project)
            .Where(e => e.Project.ProjectCode == code)
            .ToList();

        public ProjectAnnouncement? GetById(Guid id)
            => _context.ProjectAnnouncements
            .Include(e => e.Project)
            .FirstOrDefault(p => p.Id == id);

    }
}
