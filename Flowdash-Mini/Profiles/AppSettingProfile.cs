using AutoMapper;
using Flowdash_Mini.Classes;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Models.AppSettings;
using Flowdash_Mini.ViewModels.Settings;

namespace Flowdash_Mini.Profiles
{
    public class AppSettingProfile : Profile
    {
        public AppSettingProfile()
        {
            CreateMap<AppSetting, AppSettingVM>()
                .ReverseMap();

            CreateMap<IEnumerable<AppSetting>, AppSettingInfoVM>()
                .ForMember(e => e.Tel, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.Tel)).Value))
                .ForMember(e => e.Email, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.Email)).Value))
                .ForMember(e => e.ContactEmail, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.ContactEmail)).Value))
                .ReverseMap();

            CreateMap<IEnumerable<AppSetting>, AppSettingSocialVM>()
                .ForMember(e => e.Youtube, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.Youtube)).Value))
                .ForMember(e => e.Tiktok, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.Tiktok)).Value))
                .ForMember(e => e.Facebook, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.Facebook)).Value))
                .ForMember(e => e.WhatsApp, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.WhatsApp)).Value))
                .ForMember(e => e.Instagram, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.Instagram)).Value))
                .ForMember(e => e.LinkedIn, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.LinkedIn)).Value))
                .ReverseMap();

            CreateMap<IEnumerable<AppSetting>, AppSettingSmtpVM>()
                .ForMember(e => e.SmtpPort, e => e.MapFrom(e => Convert.ToInt32(e.First(e => e.Key == nameof(AppSettingKeys.SmtpPort)).Value)))
                .ForMember(e => e.SmtpSsl, e => e.MapFrom(e => Convert.ToBoolean(e.First(e => e.Key == nameof(AppSettingKeys.SmtpSsl)).Value)))
                .ForMember(e => e.SmtpPassword, e => e.MapFrom(e => Ciphering.Decrypt(e.First(e => e.Key == nameof(AppSettingKeys.SmtpPassword)).Value)))
                .ForMember(e => e.SmtpName, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.SmtpName)).Value))
                .ForMember(e => e.SmtpHost, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.SmtpHost)).Value))
                .ForMember(e => e.SmtpEmail, e => e.MapFrom(e => e.First(e => e.Key == nameof(AppSettingKeys.SmtpEmail)).Value))
                .ReverseMap();
        }
    }
}
