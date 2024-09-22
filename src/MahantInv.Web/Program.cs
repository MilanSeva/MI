using Autofac.Extensions.DependencyInjection;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Identity;
using MahantInv.Infrastructure.SeedScripts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace MahantInv.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Information()
           .WriteTo.Debug(new RenderedCompactJsonFormatter())
           .WriteTo.File("logs\\logs.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

            
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<MIDbContext>();
                    //                    context.Database.Migrate();
                    context.Database.EnsureCreated();
                    //SeedData.Initialize(services);

                    var userManager = services.GetRequiredService<UserManager<MIIdentityUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await SeedData.Initialize(services, userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureWebHostDefaults(webBuilder =>
        {
            
            webBuilder
                .UseStartup<Startup>();
            Log.Logger.Error("Prod");
        });

    }
}
