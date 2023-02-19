using Microsoft.AspNetCore.Hosting;
using SwitchBook.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace SwitchBook.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => { });
    }
}