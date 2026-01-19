using AutoMapper;
using Flowdash_Mini.Models.TaskBoards;
using Flowdash_Mini.ViewModels.TaskBoards;
using Flowdash_Mini.ViewModels.Tasks;

namespace Flowdash_Mini.Profiles
{
    public class TaskBoardProfile : Profile
    {
        public TaskBoardProfile()
        {
            CreateMap<AppTaskBoard, TaskBoardVM>().ReverseMap();
            CreateMap<AppTaskBoard, CreateTaskBoardVM>().ReverseMap();
            CreateMap<AppTaskBoard, EditTaskBoardVM>().ReverseMap();

            CreateMap<AppTask, TaskVM>().ReverseMap();
        }
    }
}
