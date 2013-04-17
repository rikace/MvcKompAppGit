using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MvcKomp3.Infrastructure
{
    public abstract class RssActionResult : ActionResult
    {

    }

    public class RssActionResult<T> : RssActionResult
    {

        public RssActionResult(string title, IEnumerable<T> data,
            Func<T, XElement> formatter)
        {

            Title = title;
            DataItems = data;
            Formatter = formatter;
        }

        public IEnumerable<T> DataItems { get; set; }
        public Func<T, XElement> Formatter { get; set; }
        public string Title { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {

            HttpResponseBase response = context.HttpContext.Response;

            // set the content type of the response
            response.ContentType = "application/rss+xml";
            // get the RSS content
            string rss = GenerateXML(response.ContentEncoding.WebName);
            // write the content to the client
            response.Write(rss);
        }

        private string GenerateXML(string encoding)
        {

            XDocument rss = new XDocument(new XDeclaration("1.0", encoding, "yes"),
               new XElement("rss", new XAttribute("version", "2.0"),
                   new XElement("channel", new XElement("title", Title),
                       DataItems.Select(e => Formatter(e)))));

            return rss.ToString();
        }
    }

    // public RssActionResult RSS() {

    //        StoryLink[] stories = GetAllStories();
    //        return new RssActionResult<StoryLink>("My Stories", stories, e => {
    //            return new XElement("item",
    //                new XAttribute("title", e.Title),
    //                new XAttribute("description", e.Description),
    //                new XAttribute("link", e.Url));
    //        });
    //    }




    //    [HttpPost]
    //    public JsonResult JsonData() {

    //        StoryLink[] stories = GetAllStories();
    //        return Json(stories);
    //    }


    //    private StoryLink[] GetAllStories() {
    //        return new StoryLink[] {
    //            new StoryLink {
    //                Title = "First example story",
    //                Description = "This is the first example story",
    //                Url = "/Story/1"},
    //            new StoryLink {
    //                Title = "Second example story",
    //                Description = "This is the second example story",
    //                Url = "/Story/2"},
    //            new StoryLink {
    //                Title = "Third example story",
    //                Description = "This is the third example story",
    //                Url = "/Story/3"},
    //        };
    //    }


    //}

    //public class StoryLink {
    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    public string Url { get; set; }
    //}
}