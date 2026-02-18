using Flowdash_Mini.ViewModels.Notifications;
using Flowdash_Mini.ViewModels.Settings;

namespace Flowdash_Mini.ViewModels.Views
{
    public class HeaderVM
    {
        public AppSettingSocialVM? Socials { get; set; }
        public AppSettingInfoVM? Info { get; set; }
        public List<NotificationVM> Notifications { get; set; } = new();
    }
}
