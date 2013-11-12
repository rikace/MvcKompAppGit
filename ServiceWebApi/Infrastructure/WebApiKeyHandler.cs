using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ServiceWebApi.Infrastructure
{
    public class WebApiKeyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
           // return base.SendAsync(request, cancellationToken);
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

    public class WebApiKey
    {
        internal static bool IsValid(string apikey)
        {
            return APIKeyRepository.IsValidAPIKey(apikey);
        }
    }

    public static class APIKeyRepository
    {
        public static bool IsValidAPIKey(string key)
        {
            // TODO: Implement IsValidAPI Key using your repository

            Guid apiKey;

            // Convert the string into a Guid and validate it
            if (Guid.TryParse(key, out apiKey) && APIKeys.Contains(apiKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static List<Guid> APIKeys
        {
            get
            {
                // Get from the cache
                // Could also use AppFabric cache for scalability
                var keys = HttpContext.Current.Cache[APIKEYLIST] as List<Guid>;

                if (keys == null)
                    keys = PopulateAPIKeys();

                return keys;
            }
        }

        private static List<Guid> PopulateAPIKeys()
        {
            List<Guid> keyList = new List<Guid>();

            foreach (var key in Properties.Settings.Default.ApiKey)
            {
                keyList.Add(new Guid(key));
            }

            //DataContractSerializer dcs = new DataContractSerializer(typeof(List<Guid>));
            //var server = HttpContext.Current.Server;

            //using (FileStream fs = new FileStream(server.MapPath("~/App_Data/APIKeys.xml"), FileMode.Open))
            //using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
            //{
            //    keyList = (List<Guid>)dcs.ReadObject(reader);
            //}

            // Save it in the cache
            // Could be saved in AppFabric Cache for scalability across a farm
            HttpContext.Current.Cache[APIKEYLIST] = keyList;

            return keyList;
        }

        const string APIKEYLIST = "APIKeyList";

    }
}