using System;
using System.Web.Mvc;

namespace BookSamples.Components.ActionResults
{
    public static class JsonpExtensions
    {
        /*
         * The basic idea of JSONP is retrieving JSON data via script tags. Script tags work around
         * same-origin policy limitations. So you point to a remote cross-domain JSON source and pass
         * an extra parameter (defined by the endpoint) so that the site return a call to your function 
         * which receives JSON data. Simply pointing the SCRIPT tag to the JSON URL would work but 
         * it would get you just data, and when evaluated within the browser, it has no externally detectable effect.
         */
        public static JsonpResult Jsonp(this Controller controller, Object data, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return new JsonpResult
            {
                Data = data,
                JsonRequestBehavior = behavior
            };
        }
    }
}