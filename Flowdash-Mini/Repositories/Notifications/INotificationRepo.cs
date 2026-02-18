using Flowdash_Mini.Models.Accounts;

namespace Flowdash_Mini.Repositories.Notifications
{
    public interface INotificationRepo
    {
        void Add(Notification item);
        void PushNotification(Guid userId, string text);
        Notification? GetById(Guid id);
        IQueryable<Notification> GetAll(Guid userId);
        IQueryable<Notification> GetAll();
        void Delete(Guid id);
    }
}
