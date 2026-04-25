using Flowdash_Mini.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.InitializeDatabase(config);

builder.Services.InitializeAuthentication(config);

builder.Services.AddApplicationServices(config);

builder.Services.ConfigureApiSupport();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
    app.UseWhen(context
        => !context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
        apiApp =>
        {
            apiApp.UseStatusCodePagesWithReExecute("/Error/{0}");
            apiApp.UseExceptionHandler("/Error");
        });
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flowdash-Mini Api v1");
    });
}

await app.MigrateDatabaseAsync();
app.ConfigureDateFormat("dd/MM/yyyy");

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
