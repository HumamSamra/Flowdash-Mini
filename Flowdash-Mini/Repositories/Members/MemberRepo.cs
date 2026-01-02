
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
    }
}
