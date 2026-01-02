using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Models.AppSettings;
using Flowdash_Mini.Models.Files;
using Flowdash_Mini.Models.Projects;
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
        public DbSet<ProjectMember> ProjectMembers { get; set; }

        public DbSet<AppSetting> AppSettings { get; set; }
    }
}
