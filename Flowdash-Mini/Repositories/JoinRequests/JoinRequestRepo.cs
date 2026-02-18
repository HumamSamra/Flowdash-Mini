using Flowdash_Mini.Context;
using Flowdash_Mini.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Repositories.JoinRequests
{
    public class JoinRequestRepo : IJoinRequestRepo
    {
        private readonly AppDbContext _context;
        public JoinRequestRepo(AppDbContext context)
        {
            _context = context;
        }

        public void Add(ProjectJoinRequest item)
        {
            item.CreatedAt = DateTime.UtcNow;
            _context.ProjectJoinRequests.Add(item);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var item = Get(id);
            if (item != null)
            {
                _context.ProjectJoinRequests.Remove(item);
                _context.SaveChanges();
            }
        }

        public ProjectJoinRequest? Get(Guid id)
            => _context.ProjectJoinRequests
            .Include(e => e.User)
            .Include(e => e.Project)
            .FirstOrDefault(r => r.Id == id);

        public IQueryable<ProjectJoinRequest> GetAll(Guid projectId)
            => _context.ProjectJoinRequests
            .Include(e => e.Project)
            .Include(e => e.User)
            .Where(e => e.ProjectId == projectId)
            .AsQueryable();

        public IQueryable<ProjectJoinRequest> GetAll(string projectCode)
            => _context.ProjectJoinRequests
            .Include(e => e.Project)
            .Include(e => e.User)
            .Where(e => e.Project.ProjectCode == projectCode)
            .AsQueryable();
    }
}
