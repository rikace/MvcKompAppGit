using System;
using System.Web;

namespace SurfStoreApp.Utils
{
    //<modules>
    //  <add name="ETagUtils" type="SurfStoreApp.Utils.ETagUtils" />
    //</modules>

    public class ETagUtils : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.PostReleaseRequestState += application_PostReleaseRequestState;

        }

        public void Dispose()
        {
        }

        void application_PostReleaseRequestState(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("ETag");
        }
    }
}