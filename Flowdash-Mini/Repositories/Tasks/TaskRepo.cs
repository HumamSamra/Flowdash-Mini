using Flowdash_Mini.Context;
using Flowdash_Mini.Models.TaskBoards;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Repositories.Tasks
{
    public class TaskRepo : ITaskRepo
    {
        private readonly AppDbContext _context;
        public TaskRepo(AppDbContext context)
        {
            _context = context;
        }

        public void Create(AppTask item)
        {
            item.CreatedAt = DateTime.UtcNow;
            item.ModifiedAt = DateTime.UtcNow;

            _context.AppTasks.Add(item);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _context.AppTasks.Remove(item);
                _context.SaveChanges();
            }
        }

        public IQueryable<AppTask> GetAll()
            => _context.AppTasks.AsQueryable();

        public AppTask? GetById(Guid id)
            => _context.AppTasks
            .Include(e => e.TaskBoard)
            .FirstOrDefault(p => p.Id == id);

        public void Update(AppTask modifiedItem)
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
