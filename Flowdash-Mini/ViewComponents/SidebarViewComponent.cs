using AutoMapper;
using Flowdash_Mini.Repositories;
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
            return View();
        }
    }
}
