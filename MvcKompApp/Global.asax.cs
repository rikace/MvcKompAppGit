using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using MvcKompApp.DAL;
using MvcKompApp.Infrastructure;
using MvcKompApp.Models;
using MvcKompApp.Validation;

namespace MvcKompApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
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
            Database.SetInitializer<SchoolContext>(new SchoolInitializer());
          //  Database.SetInitializer<ImageContext>(new ImageInitializer());
            Database.SetInitializer<MovieContext>(new MovieInitializer());
#endif

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RemoteUID_Attribute),
                typeof(RemoteValidator));

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());

        }
    }
}