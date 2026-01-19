using Flowdash_Mini.Models.Projects;

namespace Flowdash_Mini.Repositories.Projects
{
    public interface IProjectRepo
    {
        Project? GetById(Guid id);
        Project? GetByCode(string code);
        IQueryable<Project> GetAll();
        void Create(Project item);
        void Update(Project modifiedItem);
        void Delete(Guid id);

        IQueryable<ProjectLog> GetLogs(string code);
        IQueryable<ProjectLog> GetLogs(Guid projectId);
        void Log(ProjectLog log);
    }
}
