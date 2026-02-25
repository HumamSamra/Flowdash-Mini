using Flowdash_Mini.Models;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.Services.CaptchaService;
using Flowdash_Mini.Services.MailService;
using Flowdash_Mini.Services.StorageService;
using Flowdash_Mini.Services.TokenService;
using System.Reflection;

namespace Flowdash_Mini.Extensions
{
    public static class ServiceExtension
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSession();
            services.AddAutoMapper(e => { }, Assembly.GetExecutingAssembly());
            services.AddScoped<ICaptchaService, CaptchaService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ISmtpService, SmtpService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();

            services.Configure<CaptchaOptions>(
                config.GetSection("Captcha"));

            services.AddCors(options =>
            {
                options.AddPolicy("DefaultOrigin",
                    b =>
                    {
                        b.WithOrigins(config["WebDomain"]!)
                         .AllowAnyHeader()
                         .AllowAnyMethod();
                    });
            });
        }
    }
}
