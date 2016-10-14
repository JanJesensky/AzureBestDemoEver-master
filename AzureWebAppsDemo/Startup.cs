using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureWebAppsDemo.Startup))]
namespace AzureWebAppsDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
