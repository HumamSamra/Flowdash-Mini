using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Models.AppSettings;
using Flowdash_Mini.Models.Files;
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

        public DbSet<AppSetting> AppSettings { get; set; }
    }
}
