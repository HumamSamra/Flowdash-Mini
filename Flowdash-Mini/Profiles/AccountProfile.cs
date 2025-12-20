using AutoMapper;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.ViewModels.Accounts;

namespace Flowdash_Mini.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AppUser, RegisterVM>().ReverseMap();
            CreateMap<AppUser, LoginVM>().ReverseMap();
            CreateMap<AppUser, AppUserVM>().ReverseMap();
            CreateMap<AppUser, EditUserVM>().ReverseMap();
            CreateMap<AppUser, CreateUserVM>().ReverseMap();
        }
    }
}
