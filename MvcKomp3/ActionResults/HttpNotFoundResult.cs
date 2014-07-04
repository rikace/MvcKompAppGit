using System;
using System.Web.Mvc;

namespace BookSamples.Components.ActionResults
{
    public class HttpNotFoundResult : HttpStatusCodeResult
    { 
        public HttpNotFoundResult() : this(null)
        {
        }
        public HttpNotFoundResult(String statusDescription)
            : base(404, statusDescription)
        {
        }
    }
}