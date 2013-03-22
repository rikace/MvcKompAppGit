using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO.Compression;

namespace MvcKompApp.Framework
{
    public class HttpCompressionModule : IHttpModule
    {
        #region IHttpModule Members

        void IHttpModule.Dispose()
        {
            
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.PostAcquireRequestState += new EventHandler(context_PostAcquireRequestState);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            HttpApplication context = sender as HttpApplication;
            context.PostAcquireRequestState -= new EventHandler(context_PostAcquireRequestState);
            context.EndRequest -= new EventHandler(context_EndRequest);
        }

        void context_PostAcquireRequestState(object sender, EventArgs e)
        {
            this.RegisterCompressFilter();    
        }

        private void RegisterCompressFilter()
        {
            HttpContext context = HttpContext.Current;

            if (context.Handler is StaticFileHandler 
                || context.Handler is DefaultHttpHandler) return;

            HttpRequest request = context.Request;
            
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            HttpResponse response = HttpContext.Current.Response;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }

        #endregion
    }
}
