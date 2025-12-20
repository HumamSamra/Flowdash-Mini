using Flowdash_Mini.Context;
using Flowdash_Mini.Models.Accounts;
using Microsoft.AspNetCore.Identity;

namespace Flowdash_Mini.Extensions
{
    public static class AuthExtension
    {
        public static void InitializeAuthentication(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";

                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 7;
                opt.Lockout.AllowedForNewUsers = true;

                opt.Password.RequireNonAlphanumeric = false;
                opt.SignIn.RequireConfirmedEmail = true;

                opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddUserManager<UserManager<AppUser>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<SecurityStampValidatorOptions>(opt =>
            {
                opt.ValidationInterval = TimeSpan.Zero;
            });

            services.Configure<DataProtectionTokenProviderOptions>
                (options => options.TokenLifespan = TimeSpan.FromMinutes(20));

            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/auth/Login";
                opt.LogoutPath = "/auth/Logout";
                opt.AccessDeniedPath = "/";
                opt.ExpireTimeSpan = TimeSpan.FromDays(60);
                opt.SlidingExpiration = true;
            });

            services.AddAuthentication()
            .AddCookie("IdentityCookie");
        }
    }
}
