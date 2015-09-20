using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ImagesDownloader.Startup))]

namespace ImagesDownloader
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
