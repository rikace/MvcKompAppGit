using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BookSamples.Components.ActionResults
{
    public class JsonpResult : JsonResult
    {
        private const String JsonpCallbackName = "callback";

        public JsonpResult()
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if ((JsonRequestBehavior == JsonRequestBehavior.DenyGet) && 
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException();
            }

            var response = context.HttpContext.Response;
            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data == null) 
                return;

            var request = context.HttpContext.Request;
            var serializer = new JavaScriptSerializer();
            var buffer = request[JsonpCallbackName] != null 
                             ? String.Format("{0}({1})", 
                                request[JsonpCallbackName], 
                                serializer.Serialize(Data)) 
                             : serializer.Serialize(Data);
            response.Write(buffer);
        }
    }
}