using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace MvcKomp3.Infrastructure.WebApi
{
    //public class WebApiKeyHandler : DelegatingHandler
    //{
    //    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        string apikey = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("apikey");
    //        if (string.IsNullOrWhiteSpace(apikey))
    //        {
    //            HttpResponseMessage response = request.CreateErrorResponse(HttpStatusCode.Forbidden, "You can't use the API without the key.");
    //            throw new HttpResponseException(response);
    //        }
    //        else
    //        {
    //            return base.SendAsync(request, cancellationToken);
    //        }
    //    }
    //}
}