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
            .Include(e => e.TaskBoards)
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

        public void Log(ProjectLog log)
        {
            log.CreatedAt = DateTime.UtcNow;
            _context.ProjectLogs.Add(log);
            _context.SaveChanges();
        }

        public IQueryable<ProjectLog> GetLogs(string code)
        {
            return _context.ProjectLogs
                .Include(e => e.Project)
                .Where(l => l.Project.ProjectCode == code)
                .AsQueryable();
        }

        public IQueryable<ProjectLog> GetLogs(Guid projectId)
        {
            return _context.ProjectLogs
                .Include(e => e.Project)
                .Where(l => l.ProjectId == projectId)
                .AsQueryable();
        }

        public IQueryable<ProjectJoinRequest> GetProjectJoinRequests(Guid projectId)
        {
            return _context.ProjectJoinRequests
                .Include(e => e.User)
                .Include(e => e.Project)
                .Where(r => r.ProjectId == projectId)
                .AsQueryable();
        }

        public IQueryable<ProjectJoinRequest> GetUserJoinRequests(Guid userId)
        {
            return _context.ProjectJoinRequests
                .Include(e => e.User)
                .Include(e => e.Project)
                .Where(r => r.UserId == userId)
                .AsQueryable();
        }

        public bool JoinRequestExists(Guid userId, Guid projId)
        {
            return _context.ProjectJoinRequests
                .Any(r => r.UserId == userId && r.ProjectId == projId);
        }

        public void CreateProjectJoinRequest(ProjectJoinRequest item)
        {
            item.CreatedAt = DateTime.UtcNow;
            _context.ProjectJoinRequests.Add(item);
            _context.SaveChanges();
        }

        public void DeleteProjectJoinRequest(Guid id)
        {
            var item = _context.ProjectJoinRequests
                .FirstOrDefault(e => e.Id == id);
            if (item != null)
            {
                _context.ProjectJoinRequests.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}
