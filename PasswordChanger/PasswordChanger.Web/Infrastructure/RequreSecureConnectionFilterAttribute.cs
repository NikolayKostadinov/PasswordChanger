//-----------------------------------------------------------------------
// <copyright file="$safeitemname&" company="">
//     Copyright &year& (c) . All rights reserved.
// </copyright>
// <author>Nikolay.Kostadinov</author>
//-----------------------------------------------------------------------

namespace PasswordChanger.Web.Infrastructure
{
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Summary description for $classname$
    /// </summary>
    public class RequreSecureConnectionFilterAttribute : RequireHttpsAttribute
    {
        private readonly ILog logger;

        public RequreSecureConnectionFilterAttribute(ILog loggerParam)
        {
            this.logger = loggerParam;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsLocal)
            {

                if (!filterContext.HttpContext.Request.IsSecureConnection)
                {
                    int port = filterContext.HttpContext.Request.Url.Port;
                    filterContext.HttpContext.Response.RedirectPermanent(filterContext.HttpContext.Request.Url.ToString().Replace("http://", "https://").Replace($":{port.ToString()}", ":443"));
                }
            }

            base.OnAuthorization(filterContext);
        }
    }
}