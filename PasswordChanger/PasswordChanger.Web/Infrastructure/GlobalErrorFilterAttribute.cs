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
    using System.Web.Routing;

    /// <summary>
    /// Summary description for $classname$
    /// </summary>
    public class GlobalErrorFilterAttribute : FilterAttribute, IExceptionFilter
    {
        private readonly ILog logger;

        public GlobalErrorFilterAttribute(ILog loggerParam)
        : base()
        {
            this.logger = loggerParam;
        }

        public void OnException(ExceptionContext filterContext)
        {
            var parameters = filterContext.HttpContext.Request.Params;
            var details = new List<string>();
            foreach (var key in parameters.AllKeys)
            {
                details.Add(string.Format("{0} = {1}", key, parameters[key]));
            }
            this.logger.Error($"{filterContext.Exception.Message}\n{filterContext.Exception.StackTrace}\nDetails{String.Join("\n", details)}");

            filterContext.ExceptionHandled = true;

            var tempdata = new TempDataDictionary();
            if (filterContext.Controller.TempData.ContainsKey("ErrorMessage"))
            {
                filterContext.Controller.TempData["ErrorMessage"] = $"There was a problem during password changing process!!!\n Please contact service desk.\n{filterContext.Exception.Message}\n{filterContext.Exception.StackTrace}";
            }
            else
            {
                filterContext.Controller.TempData.Add("ErrorMessage", $"There was a problem during password changing process!!!\n Please contact service desk.\n{filterContext.Exception.Message}\n{filterContext.Exception.StackTrace}");
            }

            filterContext.Result = new ViewResult
            {
                ViewName = "Index",
                TempData = filterContext.Controller.TempData,
            };

        }
    }
}
