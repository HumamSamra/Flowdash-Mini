using AutoMapper;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.ViewModels.Settings;
using Flowdash_Mini.ViewModels.Views;
using Microsoft.AspNetCore.Mvc;

namespace Flowdash_Mini.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public FooterViewComponent(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var settings = _unitOfWork.AppSettings.GetAll().ToArray();
            var socials = _mapper.Map<AppSettingSocialVM>(settings);
            var info = _mapper.Map<AppSettingInfoVM>(settings);
            return View(new FooterVM() { Info = info, Socials = socials });
        }
    }
}
