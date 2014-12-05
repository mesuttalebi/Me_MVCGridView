using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(mesoft.gridview.Startup))]
namespace mesoft.gridview
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
