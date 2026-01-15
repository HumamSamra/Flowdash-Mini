using Flowdash_Mini.Models.Projects;

namespace Flowdash_Mini.Repositories.Announcements
{
    public interface IAnnouncementRepo
    {
        ProjectAnnouncement? GetById(Guid id);
        IQueryable<ProjectAnnouncement> GetAll();
        List<ProjectAnnouncement> GetAllByProjectCode(string code);
        void Create(ProjectAnnouncement item);
        void Delete(Guid id);
    }
}
