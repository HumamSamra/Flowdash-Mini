using AutoMapper;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.ViewModels.Settings;
using Flowdash_Mini.ViewModels.Views;
using Microsoft.AspNetCore.Mvc;

namespace Al_Madinah_Reparing.ViewComponents
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
            var socials = _mapper.Map<AppSettingSocialVM>(settings);
            var info = _mapper.Map<AppSettingInfoVM>(settings);
            return View(new HeaderVM() { Info = info, Socials = socials });
        }
    }
}
