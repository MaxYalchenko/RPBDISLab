[assembly: HostingStartup(typeof(CompanyApp.Areas.Identity.IdentityHostingStartup))]
namespace CompanyApp.Areas.Identity
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