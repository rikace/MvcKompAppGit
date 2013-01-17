using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class VideoController : ApiController
    {
        public HttpResponseMessage GetVideo()
        {
            var fs = File.OpenRead(@"c:\temp\Sleep Away.mp3");
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StreamContent(fs);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("audio/mp3");

            return response;

        }
    }
}