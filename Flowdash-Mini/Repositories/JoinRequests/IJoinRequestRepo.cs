using Flowdash_Mini.Models.Projects;

namespace Flowdash_Mini.Repositories.JoinRequests
{
    public interface IJoinRequestRepo
    {
        IQueryable<ProjectJoinRequest> GetAll(Guid projectId);
        IQueryable<ProjectJoinRequest> GetAll(string projectCode);
        ProjectJoinRequest? Get(Guid id);
        ProjectJoinRequest? GetByUserId(Guid userId, string projectCode);
        void Add(ProjectJoinRequest item);
        void Delete(Guid id);
    }
}
