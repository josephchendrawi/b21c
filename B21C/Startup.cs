using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(B21C.Startup))]
namespace B21C
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
