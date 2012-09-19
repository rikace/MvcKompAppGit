﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCNestedModels
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}/{ViewHint}", // URL with parameters
                new { controller = "Account", action = "Register", id = UrlParameter.Optional, ViewHint = UrlParameter.Optional} // Parameter defaults
            );

        }

        protected void Application_Start()
        {

            MVCControlsToolkit.Core.Extensions.Register();//this is the line of code to add
           
            AreaRegistration.RegisterAllAreas();
            MVCControlsToolkit.Core.ClientDataTypeModelValidatorProviderExt.ErrorMessageResources = typeof(MVCNestedModels.Models.Resource1);
            MVCControlsToolkit.Core.ClientDataTypeModelValidatorProviderExt.NumericErrorKey = "NumericError";
            MVCControlsToolkit.Core.ClientDataTypeModelValidatorProviderExt.DateTimeErrorKey = "DateError";
            RegisterRoutes(RouteTable.Routes); 
        }
    }
}