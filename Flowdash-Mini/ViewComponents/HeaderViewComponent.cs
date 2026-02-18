using AutoMapper;
using Flowdash_Mini.Extensions;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.ViewModels.Notifications;
using Flowdash_Mini.ViewModels.Settings;
using Flowdash_Mini.ViewModels.Views;
using Microsoft.AspNetCore.Mvc;

namespace Flowdash_Mini.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public HeaderViewComponent(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var settings = _unitOfWork.AppSettings.GetAll().ToArray();
            var notifications = _unitOfWork.Notifications
                .GetAll(HttpContext.User.GetUserId())
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
            var socials = _mapper.Map<AppSettingSocialVM>(settings);
            var info = _mapper.Map<AppSettingInfoVM>(settings);
            return View(new HeaderVM()
            {
                Info = info,
                Socials = socials,
                Notifications = _mapper.Map<List<NotificationVM>>(notifications)
            });
        }
    }
}
