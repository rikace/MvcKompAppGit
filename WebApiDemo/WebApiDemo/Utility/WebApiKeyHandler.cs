using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace  WebApiDemo.Utility
{
    public class WebApiKeyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
            //IEnumerable<string> apiKeyHeaderValues = null;
            //if (request.Headers.TryGetValues("X-ApiKey", out apiKeyHeaderValues))
            //{
            //    var apiKeyHeaderValue = apiKeyHeaderValues.First();

            //    // ... your authentication logic here ...
            //    var username = (apiKeyHeaderValue == "12345" ? "Maarten" : "OtherUser");

            //    var usernameClaim = new Claim(ClaimTypes.Name, username);
            //    var identity = new ClaimsIdentity(new[] { usernameClaim }, "ApiKey");
            //    var principal = new ClaimsPrincipal(identity);

            //    Thread.CurrentPrincipal = principal;
            //}

            string apikey = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("apikey");
            if (string.IsNullOrWhiteSpace(apikey))
            {
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Forbidden, "You can't use the API without the key.");
                throw new HttpResponseException(response);
            }
            else if (WebApiKey.IsValid(apikey))
            {
                return base.SendAsync(request, cancellationToken);
            }
            else
                return SendError("The Key provided is invalid", HttpStatusCode.Forbidden);
        }

        private Task<HttpResponseMessage> SendError(string error, HttpStatusCode code)
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(error);
            response.StatusCode = code;

            return Task<HttpResponseMessage>.Factory.StartNew(() => response);
        }
    }
}