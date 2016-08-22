//-----------------------------------------------------------------------
// <copyright file="TestController" company="">
//     Copyright &year& (c) . All rights reserved.
// </copyright>
// <author>Nikolay.Kostadinov</author>
//-----------------------------------------------------------------------

namespace PasswordChanger.WebApiService.Controllers
{
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;


    /// <summary>
    /// Summary description for TestsController
    /// </summary>
    public class TestsController : ApiController
    {
        private readonly ILog logger;

        public TestsController(ILog loggerParam)
        {
            this.logger = loggerParam;
        }

        [Route("api/Tests/GetTest")]
        [HttpGet]
        public HttpResponseMessage GetTest()
        {
            return Request.CreateResponse(System.Net.HttpStatusCode.OK,$"Now everything is perfect!");
        }
    }
}
