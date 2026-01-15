using Flowdash_Mini.Models.TaskBoards;

namespace Flowdash_Mini.Repositories.TaskBoards
{
    public interface ITaskBoardRepo
    {
        AppTaskBoard? GetById(Guid id);
        IQueryable<AppTaskBoard> GetAll();
        void Create(AppTaskBoard item);
        void Update(AppTaskBoard modifiedItem);
        void Delete(Guid id);
    }
}
