//-----------------------------------------------------------------------
// <copyright file="AllawIpToConnectAttribute" company="">
//     Copyright &year& (c) . All rights reserved.
// </copyright>
// <author>Nikolay.Kostadinov</author>
//-----------------------------------------------------------------------

namespace PasswordChanger.WebApiService.Infrastructure
{
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// Summary description for $classname$
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AllawIpToConnectAttribute : AuthorizationFilterAttribute
    {
        private readonly ILog logger;
        public AllawIpToConnectAttribute(ILog loggerParam)
        {
            this.logger = loggerParam;
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (Challenge(actionContext))
            {
                base.OnAuthorization(actionContext);
            }
            else
            {
                logger.Error($"Invalid attempt to access from IP Address: {this.GetClientIp(actionContext.Request) ?? "No IP Address Available"}");
                var host = actionContext.Request.RequestUri.DnsSafeHost;
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }
        }

        private bool Challenge(HttpActionContext actionContext)
        {
            var authorizedIps = Properties.Settings.Default.allawedIPs;
            var userIp = this.GetClientIp(actionContext.Request);

            if (authorizedIps.Contains(userIp))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper) request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty) request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else
            {
                return null;
            }
        }
    }
}
