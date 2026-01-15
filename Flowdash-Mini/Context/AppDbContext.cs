using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Models.AppSettings;
using Flowdash_Mini.Models.Files;
using Flowdash_Mini.Models.Projects;
using Flowdash_Mini.Models.TaskBoards;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<DbFile> DbFiles { get; set; }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAnnouncement> ProjectAnnouncements { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<ProjectJoinRequest> ProjectJoinRequests { get; set; }
        public DbSet<ProjectInvite> ProjectInvites { get; set; }
        public DbSet<ProjectLog> ProjectLogs { get; set; }

        public DbSet<AppTaskBoard> AppTaskBoards { get; set; }
        public DbSet<AppTask> AppTasks { get; set; }

        public DbSet<AppSetting> AppSettings { get; set; }
    }
}
