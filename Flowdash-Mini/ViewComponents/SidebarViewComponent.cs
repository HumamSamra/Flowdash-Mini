using AutoMapper;
using Flowdash_Mini.Classes;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.ViewModels.Views;
using Microsoft.AspNetCore.Mvc;

namespace Flowdash_Mini.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SidebarViewComponent(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var code = CookieHandler.Get("MEMORY_PROJECT_CODE", HttpContext);
            var model = new SidebarVM()
            {
                ProjectCode = code ?? "",
            };

            var announcements = _unitOfWork.Announcements.GetAllByProjectCode(code ?? "");
            if (announcements.Count > 0)
            {
                model.LastAnnouncementId = announcements.OrderByDescending(
                    e => e.CreatedAt).First().Id.ToString();
            }
            return View(model);
        }
    }
}
