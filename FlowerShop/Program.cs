using System.Reflection;
using FlowerShop.Infrastructure.Extensions;
using FlowerShop.Web.Helpers;
using FlowerShop.Web.Services.Interfaces;
using FlowerShop.Web.Services.Mock;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Serilog.Debugging;

SelfLog.Enable(Console.Error);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddScoped<IProductService, MockProductService>()
    .AddScoped<ICategoryService, MockCategoryService>()
    .AddScoped<IOccasionService, MockOccasionService>();

builder.Services.AddControllersWithViews()
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

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