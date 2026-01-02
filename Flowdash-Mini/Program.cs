using Flowdash_Mini.Extensions;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.InitializeDatabase(config);

builder.Services.InitializeAuthentication();

builder.Services.AddApplicationServices(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseExceptionHandler("/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

await app.MigrateDatabaseAsync();

var defaultDateCulture = "en-GB";
var ci = new CultureInfo(defaultDateCulture);

ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
ci.DateTimeFormat.DateSeparator = "/";

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ci),
    SupportedCultures = new List<CultureInfo> { ci },
    SupportedUICultures = new List<CultureInfo> { ci }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("DefaultOrigin");
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
