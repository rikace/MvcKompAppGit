using MvcKomp3.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MvcKomp3.Controllers
{
    public class RssController : Controller
    {

        public RssActionResult RSS()
        {

            StoryLink[] stories = GetAllStories();
            return new RssActionResult<StoryLink>("My Stories", stories, e =>
            {
                return new XElement("item",
                    new XAttribute("title", e.Title),
                    new XAttribute("description", e.Description),
                    new XAttribute("link", e.Url));
            });
        }




        [HttpPost]
        public JsonResult JsonData()
        {

            StoryLink[] stories = GetAllStories();
            return Json(stories);
        }


        private StoryLink[] GetAllStories()
        {
            return new StoryLink[] {
                new StoryLink {
                    Title = "First example story",
                    Description = "This is the first example story",
                    Url = "/Story/1"},
                new StoryLink {
                    Title = "Second example story",
                    Description = "This is the second example story",
                    Url = "/Story/2"},
                new StoryLink {
                    Title = "Third example story",
                    Description = "This is the third example story",
                    Url = "/Story/3"},
            };
        }


    }

    public class StoryLink
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
