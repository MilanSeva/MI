using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(MahantInv.Web.Areas.Identity.IdentityHostingStartup))]
namespace MahantInv.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}