using AutoMapper;
using Flowdash_Mini.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Al_Madinah_Reparing.ViewComponents
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
            return View();
        }
    }
}
