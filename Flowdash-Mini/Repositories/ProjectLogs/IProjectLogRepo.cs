using Flowdash_Mini.Models.Projects;

namespace Flowdash_Mini.Repositories.ProjectLogs
{
    public interface IProjectLogRepo
    {
        ProjectLog? GetById(Guid id);
        IQueryable<ProjectLog> GetAll();
        void Create(ProjectLog item);
        void Delete(Guid id);
    }
}
