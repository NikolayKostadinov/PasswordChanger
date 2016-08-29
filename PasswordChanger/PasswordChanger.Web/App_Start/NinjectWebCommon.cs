[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(PasswordChanger.Web.AppStart.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(PasswordChanger.Web.AppStart.NinjectWebCommon), "Stop")]

namespace PasswordChanger.Web.AppStart
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Application.Contracts;
    using Application.Services;
    using Application.Common;
    using log4net;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger("PasswordChanger.Web")).InRequestScope();
            //kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)).InRequestScope();
            kernel.Bind<ISettingsProvider>().To<SettingsProvider>().WithConstructorArgument("settingsTypeParam", "userSettings");
            kernel.Bind<IOperationStatus>().To<OperationStatus>();
            kernel.Bind<IChangePasswordService>().To<ChangePasswordService>();
            kernel.Bind<IMailerService>().To<MailerService>();
        }
    }
}
