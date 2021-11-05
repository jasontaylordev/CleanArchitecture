using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(CleanArchitecture.WebUI.Areas.Identity.IdentityHostingStartup))]
namespace CleanArchitecture.WebUI.Areas.Identity
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