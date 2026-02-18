using AutoMapper;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.ViewModels.Notifications;

namespace Flowdash_Mini.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationVM>();
        }
    }
}
