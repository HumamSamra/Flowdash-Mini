using Flowdash_Mini.Models.Projects;

namespace Flowdash_Mini.Repositories.Members
{
    public interface IMemberRepo
    {
        ProjectMember? GetById(Guid id);
        ProjectMember? GetByUserId(Guid userId, Guid projectId);
        ProjectMember? GetByUserId(Guid userId, string projectCode);
        IQueryable<ProjectMember> GetAllByProjectId(Guid projectId);
        IQueryable<ProjectMember> GetAllByProjectCode(string code);

        void Update(ProjectMember member);
        void Delete(Guid id);
    }
}
