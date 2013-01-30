using System;
using System.Configuration;
using System.Data.Entity;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Routing;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using Log;
using MvcKomp3.Infrastructure;
using MvcKompApp.DAL;
using MvcKompApp.Filters;
using MvcKompApp.Infrastructure;
using MvcKompApp.Models;
using MvcKompApp.Validation;
using Ninject;

namespace MvcKompApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new LogonAuthorize());
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                null,
                "Employee/Page{page}",
                new { controller = "Employee", action = "Index", id = UrlParameter.Optional },
                new { page = @"\d+" });

            routes.MapRoute(
                "Employee",
                "Employee/{action}/{id}",
                new { controller = "Employee", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new
                {
                    httpMethod = new HttpMethodConstraint("GET", "POST")
                }
                // Parameter defaults
            );

        }

        protected void Application_Start()
        {
#if DEBUG
            Database.SetInitializer<TaskDBContext>(new TaskDbContextInitializer());
            using (var init = new TaskDBContext())
            {
                init.Database.Initialize(force: false);
            }


            Database.SetInitializer<SchoolContext>(new SchoolInitializer());
          //  Database.SetInitializer<ImageContext>(new ImageInitializer());
            Database.SetInitializer<MovieContext>(new MovieInitializer());

            Log.SingletonLogger.Instance.Attach(new Log.ObserverLogToConsole());

#endif
            //ModelBinders.Binders.Add(typeof(Appointment), new ValidatingModelBinder());

            //ModelValidatorProviders.Providers.Clear();
            //ModelValidatorProviders.Providers.Add(new CustomValidationProvider());

            //HtmlHelper.ClientValidationEnabled = true;
            //HtmlHelper.UnobtrusiveJavaScriptEnabled = true;

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RemoteUID_Attribute),
                typeof(RemoteValidator));

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());

            ModelBinders.Binders.Add(typeof(Appointment), new ValidatingModelBinder());

           //odelBinders.Binders.Add(typeof(DateTime), new DateBinder());
        }

        private void SetupDependency()
        {
            IKernel kernel = new StandardKernel();
            //kernel.Bind
            // Bind<IMessageProvider>().To<SimpleMessageProvider>();
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }


        /// <summary>
        /// Initializes logging facility with severity level and observer(s).
        /// Private helper method.
        /// </summary>
        private void InitializeLogger()
        {
            // Read and assign application wide logging severity
            //string severity = ConfigurationManager.AppSettings.Get("LogSeverity");
            //SingletonLogger.Instance.Severity =  (LogSeverity)Enum.Parse(typeof(LogSeverity), severity, true);

            SingletonLogger.Instance.Severity = LogSeverity.Debug | LogSeverity.Error | LogSeverity.Fatal | LogSeverity.Info;

            // Send log messages to debugger console (output window). 
            // Btw: the attach operation is the Observer pattern.
            Log.ILog log = new ObserverLogToConsole();
            SingletonLogger.Instance.Attach(log);

            // Send log messages to email (observer pattern)
            string from = "notification@yourcompany.com";
            string to = "webmaster@yourcompany.com";
            string subject = "Webmaster: please review";
            string body = "email text";
            var smtpClient = new SmtpClient("mail.yourcompany.com");
            log = new ObserverLogToEmail(from, to, subject, body, smtpClient);
            SingletonLogger.Instance.Attach(log);

            // Other log output options

            //// Send log messages to a file
            //log = new ObserverLogToFile(@"C:\Temp\DoFactory.log");
            //SingletonLogger.Instance.Attach(log);

            //// Send log message to event log
            //log = new ObserverLogToEventlog();
            //SingletonLogger.Instance.Attach(log);

            //// Send log messages to database (observer pattern)
            //log = new ObserverLogToDatabase();
            //SingletonLogger.Instance.Attach(log);
        }

    }
}