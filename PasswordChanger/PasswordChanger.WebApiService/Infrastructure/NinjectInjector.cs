//---------------------------------------------------------------------------------
// <copyright file="NinjectInjector.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace PasswordChanger.WebApiService.Infrastructure
{
    using System;
    using System.Linq;
    using System.Web.Http.Dispatcher;
    using System.Web.Http.Tracing;
    using Ninject.Web.Common;
    using log4net;
    using Ninject;
    using Application.Contracts;
    using Application.Services;


    /// <summary>
    /// Dependency injector provider
    /// </summary>
    public class NinjectInjector
    {
        private static readonly object lockObj = new object();
        private static NinjectInjector injector;
        private static IKernel kernel;

        /// <summary>
        /// Prevents a default instance of the <see cref="NinjectInjector" /> class from being created.
        /// </summary>
        private NinjectInjector()
        {
            kernel = new StandardKernel();
            InitializeKernel(kernel);
        }

        public static IKernel GetInjector
        {
            get
            {
                if (injector == null)
                {
                    lock (lockObj)
                    {
                        if (injector == null)
                        {
                            injector = new NinjectInjector();
                        }
                    }
                }

                return kernel;
            }
        }

        /// <summary>
        /// Initializes the kernel.
        /// </summary>
        public static void InitializeKernel(IKernel kernel)
        {
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger("PasswordChanger.WebApiService")).InRequestScope();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger("WebApiTrace")).WhenInjectedExactlyInto(typeof(CustomTraceWriter)).InRequestScope();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger("WebApiTrace")).WhenInjectedExactlyInto(typeof(LogRequestAndResponseHandler)).InRequestScope();
            kernel.Bind<IAdAccountManagementService>().To<AdAccountManagementService>().InRequestScope();
            kernel.Bind<ITraceWriter>().To<CustomTraceWriter>();
        }
    }
}
