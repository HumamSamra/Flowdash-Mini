using Flowdash_Mini.Models.TaskBoards;

namespace Flowdash_Mini.Repositories.Tasks
{
    public interface ITaskRepo
    {
        AppTask? GetById(Guid id);
        IQueryable<AppTask> GetAll();
        void Create(AppTask item);
        void Update(AppTask modifiedItem);
        void Delete(Guid id);
    }
}
