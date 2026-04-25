using Flowdash_Mini.Enums;

namespace Flowdash_Mini.Dtos.Tasks
{
    public class UpdateTaskDto
    {
        public Guid Id { get; set; }
        public AppTaskStatus Status { get; set; }
    }
}
