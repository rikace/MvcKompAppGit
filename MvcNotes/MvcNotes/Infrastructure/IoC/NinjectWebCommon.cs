//[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Parature.PortalWeb.App_Start.NinjectWebCommon), "Start")]
//[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Parature.PortalWeb.App_Start.NinjectWebCommon), "Stop")]

//namespace Parature.PortalWeb.App_Start
//{
//    using System;
//    using System.Web;

//    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

//    using Ninject;
//    using Ninject.Web.Common;
//    using Parature.PortalWeb.Models.Portal;
//    using Parature.DTO.Idm.Session;
//    using SessionTransferHttpModule;
//    using Parature.PortalWeb.Infrastructure;
//    using System.Web.Mvc;
//    using Parature.PortalWeb.Models.Article;
//    using Parature.PortalWeb.DataAccess;
//    using Parature.PortalWeb.DataAccess;

//    public static class NinjectWebCommon
//    {
//        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

//        /// <summary>
//        /// Starts the application
//        /// </summary>
//        public static void Start()
//        {
//            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
//            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
//            bootstrapper.Initialize(CreateKernel);
//        }

//        /// <summary>
//        /// Stops the application.
//        /// </summary>
//        public static void Stop()
//        {
//            bootstrapper.ShutDown();
//        }

//        /// <summary>
//        /// Creates the kernel that will manage your application.
//        /// </summary>
//        /// <returns>The created kernel.</returns>
//        private static IKernel CreateKernel()
//        {
//            var kernel = new StandardKernel();
//            try
//            {
//                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
//                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

//                RegisterServices(kernel);

//                // usage repository = DependencyResolver.Current.GetService<IGlobalizationRepository>();
//                System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Parature.PortalWeb.Infrastructure.IoC.NinjectDependencyResolver(kernel);

//                return kernel;
//            }
//            catch
//            {
//                kernel.Dispose();
//                throw;
//            }
//        }

//        /// <summary>
//        /// Load your modules or register your services here!
//        /// </summary>
//        /// <param name="kernel">The kernel.</param>
//        private static void RegisterServices(IKernel kernel)
//        {
//            kernel.Bind<IPortalRepository>().To<PortalRepository>().InScope(c => ProcessingScope.Current);
//            kernel.Bind<IArticleRepository>().To<ArticleRepository>().InScope(c => ProcessingScope.Current);
//            kernel.Bind<IDownloadRepository>().To<DownloadRepository>().InScope(c => ProcessingScope.Current);
//            kernel.Bind<IGlobalizationRepository>().To<GlobalizationRepository>().InScope(c => ProcessingScope.Current);
//        }
//    }

//    public static class ProcessingScope
//    {
//        public static HttpContextBase Current
//        {
//            get
//            {
//                var httpContext = HttpContext.Current.ApplicationInstance.Context;
//                SessionDTO sessionDto = SessionTransferHelper.GetSessionDTO(httpContext);

//                var contextWrapper = new HttpContextWrapper(HttpContext.Current);

//                if (sessionDto != null)
//                    contextWrapper.User = new ParaturePrincipal(sessionDto);
//                else
//                    contextWrapper.Response.Redirect("/error.html");
//                return contextWrapper;
//            }
//        }
//    }
//}
