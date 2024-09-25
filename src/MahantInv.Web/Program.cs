using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MahantInv.Infrastructure;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Dtos.Product;
using MahantInv.Infrastructure.Identity;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.SeedScripts;
using MahantInv.SharedKernel.Filter;
using MahantInv.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ConfigureEndpointDefaults(io => io.Protocols = HttpProtocols.Http1AndHttp2);
//});

var services = builder.Services;
services.AddControllers(options =>
{
    options.Filters.Add<HttpGlobalExceptionFilter>();
});
services.AddControllers(options => { options.Filters.Add<HttpGlobalExceptionFilter>(); });
string connectionString = builder.Configuration.GetConnectionString("MahantInventoryDB");

services.UseSQLiteUOW(connectionString);
services.AddDbContext<MIDbContext>(
    options => options.UseSqlite(connectionString));

services.AddControllersWithViews().AddNewtonsoftJson();

services.AddRazorPages();
services.AddIdentity<MIIdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<MIDbContext>()
    .AddDefaultTokenProviders();

services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));
services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(600);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.EnableAnnotations();
});

// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
services.Configure<ServiceConfig>(config =>
{
    config.Services = new List<ServiceDescriptor>(services);

    // optional - default path to view services is /listallservices - recommended to choose your own path
    config.Path = "/listservices";
});


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});
builder.Host.UseSerilog(CreateSerilogLogger(builder.Configuration));

builder.Services.AddMediatR(options =>
options.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly())
);

builder.Services.AddTransient<IBuyersRepository, BuyersRepository>();
builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();
builder.Services.AddTransient<IPartiesRepository, PartiesRepository>();
builder.Services.AddTransient<IProductInventoryRepository, ProductInventoryRepository>();
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
builder.Services.AddTransient<IProductUsageRepository, ProductUsageRepository>();
builder.Services.AddTransient<IStorageRepository, StorageRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//builder.Host.ConfigureContainer<ContainerBuilder>(cb => cb.Populate(builder.Services));
// Register Autofac modules
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new DefaultInfrastructureModule(false));
});
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(s => s.EnableTryItOutByDefault());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "VIA.Core.API");
        s.EnableTryItOutByDefault();
    }
    );
}

using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider;
    var context = seedService.GetRequiredService<MIDbContext>();
    context.Database.EnsureCreated();

    var userManager = seedService.GetRequiredService<UserManager<MIIdentityUser>>();
    var roleManager = seedService.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedData.Initialize(seedService, userManager, roleManager);
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();

app.UseCors(MyAllowSpecificOrigins);


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();

//Serilog Configurations
Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    return new LoggerConfiguration()
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}
//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        Log.Logger = new LoggerConfiguration()
//       .MinimumLevel.Information()
//       .WriteTo.Debug(new RenderedCompactJsonFormatter())
//       .WriteTo.File("logs\\logs.txt", rollingInterval: RollingInterval.Day)
//       .CreateLogger();


//        var host = CreateHostBuilder(args).Build();

//        using (var scope = host.Services.CreateScope())
//        {
//            var services = scope.ServiceProvider;

//            try
//            {
//                var context = services.GetRequiredService<MIDbContext>();
//                //                    context.Database.Migrate();
//                context.Database.EnsureCreated();
//                //SeedData.Initialize(services);

//                var userManager = services.GetRequiredService<UserManager<MIIdentityUser>>();
//                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//                await SeedData.Initialize(services, userManager, roleManager);
//            }
//            catch (Exception ex)
//            {
//                var logger = services.GetRequiredService<ILogger<Program>>();
//                logger.LogError(ex, "An error occurred seeding the DB.");
//            }
//        }

//        await host.RunAsync();
//    }

//    public static IHostBuilder CreateHostBuilder(string[] args) =>
//Host.CreateDefaultBuilder(args)
//    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
//    .ConfigureWebHostDefaults(webBuilder =>
//    {

//        webBuilder
//            .UseStartup<Startup>();
//        Log.Logger.Error("Prod");
//    });

//}

