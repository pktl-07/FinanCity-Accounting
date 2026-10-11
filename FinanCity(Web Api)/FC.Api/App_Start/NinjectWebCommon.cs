[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(FC.Api.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(FC.Api.App_Start.NinjectWebCommon), "Stop")]

namespace FC.Api.App_Start
{
    using System;
    using System.Web;
    using System.Web.Http;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.WebApi;
    using Ninject.Web.Common.WebHost;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using FC.Api.App_Start;
    using FC.Core.Extensions;

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
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);

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
            kernel.AddEartStone(
                "FC.Api",
                "FC.Domain.Sales",
                "FC.Domain.Auth",
                "FC.Domain.Accounts",
                "FC.Domain.Version",
                "FC.Core",
                "FC.Core.Data",
                "FC.Core.Common",
                "FC.Domain.Report",
                "FC.Api.AccountAreas",
                "FC.Api.ReportAreas",
                "FC.Api.SalesAreas");
        }
    }
}