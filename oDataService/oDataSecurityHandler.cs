using System;
using System.Web;

namespace oDataService
{
    public class oDataSecurityHandler : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest +=
               new EventHandler(context_AuthenticateRequest);
        }
        void context_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            var tempApp = app;
            //if (!CustomAuthenticationProvider.Authenticate(app.Context))
            //{
            //    app.Context.Response.Status = "401 Unauthorized";
            //    app.Context.Response.StatusCode = 401;
            //    app.Context.Response.End();
            //}
        } 

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
