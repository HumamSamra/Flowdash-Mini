using Flowdash_Mini.Context;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Extensions
{
    public static class DatabaseExtension
    {
        public static void InitializeDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(
                e => e.UseSqlServer(config.GetConnectionString("DefaultConn")));
        }

        public static async Task MigrateDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                await context.Database.MigrateAsync();

                SeedData.SeedAppSettings(unitOfWork);
                await SeedData.SeedRolesAsync(roleManager);
                await SeedData.SeedUsersAsync(userManager);
            }
        }
    }
}
