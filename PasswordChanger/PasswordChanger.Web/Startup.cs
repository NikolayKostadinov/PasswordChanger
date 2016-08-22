using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PasswordChanger.Web.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace PasswordChanger.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}
