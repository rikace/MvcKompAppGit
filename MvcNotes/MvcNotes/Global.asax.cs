using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml.Serialization;
using Common.Framework;
using MvcNotes.Commands;
using MvcNotes.Events;
using MvcNotes.Infrastructure;
using MvcNotes.Infrastructure.Cache;
using MvcNotes.Logging;

namespace MvcNotes
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception == null)
                return;

            // Do something with Error
            // Log Error

            Server.ClearError();
            Response.Redirect("error");
        }

        protected void Application_Start()
        {
            //            DatabaseConfig.InitializeDatabases();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            // ModelBinders.Binders[typeof(DateTime)] = new DateModelBinder();

            // ModelBindingConfig.RegisterCustomModelBinding();

            LogFactory.LogWith(new ConsoleLogger());

            // RegisterEDA()

            var webFormViewEngine = ViewEngines.Engines.OfType<WebFormViewEngine>().FirstOrDefault();
            if (webFormViewEngine != null)
            {
                ViewEngines.Engines.Remove(webFormViewEngine);
            }

            SetCacheService(MvcNotes.Infrastructure.Cache.Cache.Instance);
        }

        private static void SetCacheService(ICache cache)
        {
            _cache = cache;
        }

        private static ICache _cache;

        public static ICache CacheService
        {
            get
            {
                return
                    _cache;
            }
        }


        public class DatabaseConfig
        {
            public static void InitializeDatabases()
            {
                //new SimpleMembershipInitializer().Initialize();

                //Database.SetInitializer(new DataContextInitializer(new MembershipContext()));
            }

            private void RegisterEDA()
            {

                ISubscriber subscriber = ServiceLocator.Subscriber;
                // Commands
                var productCommandHandlers = new PresidentCommandHandlers(ServiceLocator.EventBus,
                    ServiceLocator.EventStore);
                subscriber.RegisterHandler(WrapLogger<AddPresidentCommand>(productCommandHandlers.Execute));
                subscriber.RegisterHandler(WrapLogger<RemovePresidentCommand>(productCommandHandlers.Execute));

                // Events
                var productEventHandlers = new PresidentEventHandlers();
                subscriber.RegisterHandler(WrapLogger<PresidentAddedEvent>(productEventHandlers.Handle));
            }

            private Action<TMessage> WrapLogger<TMessage>(Action<TMessage> action)
            {
                return new LoggingExecutor<TMessage>(action).Handle;
            }
        }
    }
}