using System.Reflection;
using FlowerShop.Infrastructure.Extensions;
using FlowerShop.Infrastructure.Filters;
using FlowerShop.Infrastructure.Htmx;
using FlowerShop.Web.Helpers;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Serilog.Debugging;

SelfLog.Enable(Console.Error);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add<HtmxToastFilter>();
        options.Filters.Add<NotFoundResultFilter>();
    })
    .AddMvcOptions(options =>
    {
        options.ModelValidatorProviders.Clear();
    });

builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true; 
});

var app = builder.Build();

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiter();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

await Seeder.SeedUsers(app.Services);

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();