using Flowdash_Mini.Classes;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Models.AppSettings;
using Flowdash_Mini.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Context
{
    public static class SeedData
    {
        public static void SeedAppSettings(IUnitOfWork unitOfWork)
        {
            if (unitOfWork.AppSettings.GetAll().Any())
            {
                return;
            }

            var list = new List<AppSetting>()
            {
                new AppSetting(nameof(AppSettingKeys.SmtpEmail), "humamdaraya@gmail.com", true),
                new AppSetting(nameof(AppSettingKeys.SmtpName), "Tornado-soft NO-REPLY", true),
                new AppSetting(nameof(AppSettingKeys.SmtpHost), "smtp.gmail.com", true),
                new AppSetting(nameof(AppSettingKeys.SmtpPort), "465", true),
                new AppSetting(nameof(AppSettingKeys.SmtpSsl), "true", true),
                new AppSetting(nameof(AppSettingKeys.SmtpPassword), Ciphering.Encrypt("afewobqrplaagahr"), true, true),

                new AppSetting(nameof(AppSettingKeys.Tel), "", true),
                new AppSetting(nameof(AppSettingKeys.Email), "", true),
                new AppSetting(nameof(AppSettingKeys.ContactEmail), "", true),
                new AppSetting(nameof(AppSettingKeys.WhatsApp), "", true),
                new AppSetting(nameof(AppSettingKeys.Facebook), "", true),
                new AppSetting(nameof(AppSettingKeys.Instagram), "", true),
                new AppSetting(nameof(AppSettingKeys.Youtube), "", true),
                new AppSetting(nameof(AppSettingKeys.LinkedIn), "", true),
                new AppSetting(nameof(AppSettingKeys.Tiktok), "", true),

            };

            foreach (var item in list)
            {
                unitOfWork.AppSettings.Add(item);
            }
        }
        public static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
        {
            if (await roleManager.Roles.AnyAsync())
            {
                return;
            }

            var migration = new AppRole()
            {
                Name = nameof(UserType.Migration),
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                CreatedBy = nameof(UserType.Migration),
                ModifiedBy = nameof(UserType.Migration),
            };
            await roleManager.CreateAsync(migration);

            var user = new AppRole()
            {
                Name = nameof(UserType.User),
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                CreatedBy = nameof(UserType.Migration),
                ModifiedBy = nameof(UserType.Migration),
            };
            await roleManager.CreateAsync(user);

            var admin = new AppRole()
            {
                Name = nameof(UserType.Admin),
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                CreatedBy = nameof(UserType.Migration),
                ModifiedBy = nameof(UserType.Migration),
            };
            await roleManager.CreateAsync(admin);

        }
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync())
            {
                return;
            }

            var user = new AppUser()
            {
                FullName = "User Default",
                UserName = "User",
                Email = "User@user.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                CreatedBy = nameof(UserType.Migration),
                ModifiedBy = nameof(UserType.Migration)
            };
            await userManager.CreateAsync(user, password: "Usr123456");
            await userManager.AddToRolesAsync(user,
                new List<string>() {nameof(UserType.Migration),
                nameof(UserType.User)});

            var admin = new AppUser()
            {
                FullName = "Admin Default",
                UserName = "Admin",
                Email = "Admin@admin.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                CreatedBy = nameof(UserType.Migration),
                ModifiedBy = nameof(UserType.Migration)
            };
            var res = await userManager.CreateAsync(admin, password: "Usr123456");
            var result = await userManager.AddToRolesAsync(admin,
                new List<string>() {nameof(UserType.Migration),
                nameof(UserType.User),
                nameof(UserType.Admin)});
        }
    }
}
