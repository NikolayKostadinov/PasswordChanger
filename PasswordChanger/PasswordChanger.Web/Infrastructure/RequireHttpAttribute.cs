//-----------------------------------------------------------------------
// <copyright file="$safeitemname&" company="">
//     Copyright &year& (c) . All rights reserved.
// </copyright>
// <author>Nikolay.Kostadinov</author>
//-----------------------------------------------------------------------

namespace PasswordChanger.Web.Infrastructure
{
    using Application.Contracts;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Summary description for $classname$
    /// </summary>
    public class RequireHttpAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // If the request has arrived via HTTPS...
            if (filterContext.HttpContext.Request.IsSecureConnection)
            {
                ISettingsProvider settingsProvider = DependencyResolver.Current.GetService<ISettingsProvider>();
                string httpPort = settingsProvider.GetSettings()["httpPort"] as string;
                var url = filterContext.HttpContext.Request.Url;
                string redirectUrl = $"http://{url.Host}:{httpPort}{url.LocalPath}";
                filterContext.Result = new RedirectResult(redirectUrl);
                filterContext.Result.ExecuteResult(filterContext);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}