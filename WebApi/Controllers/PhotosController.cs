using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using AttributeRouting;
using AttributeRouting.Web.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Drawing;
using System.Drawing.Imaging;

namespace WebApi.Controllers
{
    public class PhotosController : ApiController
    {
        // GET api/<controller>/5
        public HttpResponseMessage GetPhoto()
        {
            //var fs = File.OpenRead(@"c:\temp\Bugghina.jpg");
            //HttpResponseMessage response = new HttpResponseMessage();
            //response.Content = new StreamContent(fs); // this file stream will be closed by lower layers of web api for you once the response is completed.
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

            //return response;

            var response = new HttpResponseMessage();
            using (var fs = File.OpenRead(@"c:\temp\Bugghina.JPG")) 
            {
                Image img = Image.FromStream(fs);
                using (var ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    response.Content = new ByteArrayContent(ms.ToArray());
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
            }

        }

        //[GET("GetPhoto/fileName")]
        public HttpResponseMessage GetPhotoByName(string fileName)
        {
            //var fs =  File.OpenRead(@"c:\temp\" + fileName + ".jpg");


            //HttpResponseMessage response = new HttpResponseMessage();
            //response.Content = new StreamContent(fs); // this file stream will be closed by lower layers of web api for you once the response is completed.
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

            //return response;

            var response = new HttpResponseMessage();
            using (var fs = File.OpenRead(@"c:\temp\" + fileName + ".jpg"))
            {
                Image img = Image.FromStream(fs);
                using (var ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    response.Content = new ByteArrayContent(ms.ToArray());
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
            }
        }
    }
}