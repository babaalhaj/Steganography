using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(GigHub.Areas.Identity.IdentityHostingStartup))]
namespace GigHub.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}