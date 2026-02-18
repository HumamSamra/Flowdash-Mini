using Flowdash_Mini.Context;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Models.Accounts;

namespace Flowdash_Mini.Repositories.Notifications
{
    public class NotificationRepo : INotificationRepo
    {
        private readonly AppDbContext _context;
        public NotificationRepo(AppDbContext context)
        {
            _context = context;
        }
        public void Add(Notification item)
        {
            item.CreatedAt = DateTime.UtcNow;
            _context.Notifications.Add(item);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _context.Notifications.Remove(item);
                _context.SaveChanges();
            }
        }

        public IQueryable<Notification> GetAll(Guid userId)
        {
            return _context.Notifications.Where(
                n => n.UserId == userId).AsQueryable();
        }

        public IQueryable<Notification> GetAll()
        {
            return _context.Notifications.AsQueryable();
        }

        public Notification? GetById(Guid id)
        {
            return _context.Notifications.FirstOrDefault(
                e => e.Id == id);
        }

        public void PushNotification(Guid userId, string text)
        {
            var notification = new Notification();
            notification.UserId = userId;
            notification.Text = text;
            notification.CreatedBy = nameof(UserType.System);
            notification.CreatedAt = DateTime.UtcNow;
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }
    }
}
