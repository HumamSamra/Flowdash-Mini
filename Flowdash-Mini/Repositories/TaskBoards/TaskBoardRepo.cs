using Flowdash_Mini.Context;
using Flowdash_Mini.Models.TaskBoards;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Repositories.TaskBoards
{
    public class TaskBoardRepo : ITaskBoardRepo
    {
        private readonly AppDbContext _context;
        public TaskBoardRepo(AppDbContext context)
        {
            _context = context;
        }

        public void Create(AppTaskBoard item)
        {
            item.CreatedAt = DateTime.UtcNow;
            item.ModifiedAt = DateTime.UtcNow;

            _context.AppTaskBoards.Add(item);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _context.AppTaskBoards.Remove(item);
                _context.SaveChanges();
            }
        }

        public IQueryable<AppTaskBoard> GetAll()
            => _context.AppTaskBoards.AsQueryable();

        public AppTaskBoard? GetById(Guid id)
            => _context.AppTaskBoards
            .Include(e => e.Tasks)
            .FirstOrDefault(p => p.Id == id);

        public void Update(AppTaskBoard modifiedItem)
        {
            var item = GetById(modifiedItem.Id);
            if (item != null)
            {
                item.ModifiedAt = DateTime.UtcNow;
                _context.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}
