using Flowdash_Mini.Models.Projects;

namespace Flowdash_Mini.Repositories.Members
{
    public interface IMemberRepo
    {
        IQueryable<ProjectMember> GetAllByProjectId(Guid projectId);
        IQueryable<ProjectMember> GetAllByProjectCode(string code);
    }
}
