using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace MediaService
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

            Database.SetInitializer(new MediaService.DAL.PhotosContextInitializer());
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }
    }
}