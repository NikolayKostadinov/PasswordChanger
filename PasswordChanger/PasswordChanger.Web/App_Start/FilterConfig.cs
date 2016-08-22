using log4net;
using PasswordChanger.Web.Infrastructure;
using System.Web;
using System.Web.Mvc;

namespace PasswordChanger.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            var logger = DependencyResolver.Current.GetService<ILog>();
            filters.Add(new GlobalErrorFilterAttribute(logger));
            filters.Add(new RequreSecureConnectionFilterAttribute(logger));
        }
    }
}
