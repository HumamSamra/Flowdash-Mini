using AutoMapper;
using Flowdash_Mini.Models.Projects;
using Flowdash_Mini.ViewModels.Announcements;
using Flowdash_Mini.ViewModels.Members;
using Flowdash_Mini.ViewModels.Projects;

namespace Flowdash_Mini.Profiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectVM>().ReverseMap();
            CreateMap<Project, CreateProjectVM>().ReverseMap();
            CreateMap<Project, EditProjectVM>()
                .ForMember(e => e.DateRange, e => e.MapFrom(e => $"{e.StartDate} - {e.Deadline}"))
                .ReverseMap();

            CreateMap<ProjectMember, ProjectMemberVM>().ReverseMap();
            CreateMap<ProjectMember, EditProjectMemberVM>()
                .ForMember(e => e.FullName, e => e.MapFrom(e => e.Member.FullName))
                .ReverseMap();

            CreateMap<ProjectAnnouncement, ProjectAnnouncementVM>().ReverseMap();
        }
    }
}
