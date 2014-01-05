using ServiceWebApi.Dal;
using ServiceWebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace ServiceWebApi.Controllers
{
    public class VideosController : ApiController
    {
        public IEnumerable<VideoDto> Get()
        {
            using (var ctx = new VideoContext())
                return ctx.Videos.ToList().Select(a => new VideoDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Link = new Uri(string.Format(@"http://www.riscanet.com/Api/Videos/{0}?apikey={1}",
                    a.Id, HttpContext.Current.Request.QueryString["apikey"]))
                });
        }

        public HttpResponseMessage Get(int id)
        {
            using (var db = new VideoContext())
            {
                var response = Request.CreateResponse();

                var video = (from a in db.Videos
                             where a.Id == id
                             select a).FirstOrDefault();
                if (video != null)
                {
                    using (new NetworkConnection())
                    {
                        var fullPath = video.Path;
                        var mediaStreaming = new MediaStreaming(fullPath);
                        response.Content = new PushStreamContent(mediaStreaming.WriteToStream, new MediaTypeHeaderValue(MediaContentHelper.GetContentTypeFromFileName(video.Path)));

                    }
                }
                return response;
            }
        }
    }
}
