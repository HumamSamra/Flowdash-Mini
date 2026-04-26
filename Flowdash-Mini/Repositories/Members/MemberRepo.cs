using Flowdash_Mini.Context;
using Flowdash_Mini.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Repositories.Members
{
    public class MemberRepo : IMemberRepo
    {
        private readonly AppDbContext _context;
        public MemberRepo(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<ProjectMember> GetAllByProjectId(Guid projectId)
            => _context.ProjectMembers
            .Include(e => e.Project)
            .Where(e => e.ProjectId == projectId)
            .AsQueryable();

        public IQueryable<ProjectMember> GetAllByProjectCode(string code)
            => _context.ProjectMembers
            .Include(e => e.Project)
            .Where(e => e.Project.ProjectCode == code)
            .AsQueryable();

        public ProjectMember? GetById(Guid id)
            => _context.ProjectMembers
            .Include(e => e.Project)
            .Include(e => e.Member)
            .FirstOrDefault(e => e.Id == id);

        public ProjectMember? GetByUserId(Guid userId, Guid projectId)
            => _context.ProjectMembers
            .Include(e => e.Project)
            .FirstOrDefault(e => e.MemberId == userId
                && e.ProjectId == projectId);

        public ProjectMember? GetByUserId(Guid userId, string projectCode)
            => _context.ProjectMembers
            .Include(e => e.Project)
            .Include(e => e.Member)
            .FirstOrDefault(e => e.MemberId == userId
                && e.Project.ProjectCode == projectCode);

        public void Update(ProjectMember member)
        {
            var item = GetById(member.Id);
            if (item != null)
            {
                _context.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            var member = GetById(id);
            if (member != null)
            {
                _context.ProjectMembers.Remove(member);
                _context.SaveChanges();
            }
        }
    }
}
