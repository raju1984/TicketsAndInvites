using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EventManager1.Startup))]
namespace EventManager1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
