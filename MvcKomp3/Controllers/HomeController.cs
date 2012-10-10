using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MvcKomp3.Infrastructure;
using MvcKompApp.Models;

namespace MvcKompApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int? id)
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            ItemToSelect[] items = new ItemToSelect[] { 
                new ItemToSelect{ Id=0, Name ="Employee"},
                new ItemToSelect{ Id=1, Name ="Course"},
                new ItemToSelect{ Id=2, Name ="Department"},
                new ItemToSelect{ Id=3, Name ="Instructor"},
                new ItemToSelect{ Id=4, Name ="Product"},
                new ItemToSelect{ Id=5, Name ="Person"},
                new ItemToSelect{ Id=6, Name ="User"}
            };

            ViewBag.SelectedControl = new SelectList(items, "Id", "Name", id);

            return View();
        }

        private class ItemToSelect
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult PartialViewRefreshHandler()
        {
            return View();
        }

        public JsonResult CheckUsername(string username)
        {
            var result = false;

            if (username == "sallen")
            {
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult RefreshPartial(FormCollection form)
        {
            string name = form["Name"];

            ViewBag.Name = name == null ? "No one, just refreshing" : name;
            ViewBag.DateTime = DateTime.Now.ToString();

            return PartialView("_DateTimePartial");
        }

        public ActionResult WebGridIndex()
        {
            var mostPopular = GetFavouriteGiveNames();
            return View(mostPopular);
        }

        private static List<FavouriteGivenName> GetFavouriteGiveNames()
        {
            var mostPopular = new List<FavouriteGivenName>()
            {
                 new FavouriteGivenName() {Id = 1, Name = "Jack", Age = 30},
                new FavouriteGivenName() {Id = 2, Name = "Riley", Age = 40},
                new FavouriteGivenName() {Id = 3, Name = "William", Age = 17},
                new FavouriteGivenName() {Id = 4, Name = "Oliver", Age = 56},
                new FavouriteGivenName() {Id = 5, Name = "Lachlan", Age = 25},
                new FavouriteGivenName() {Id = 6, Name = "Thomas", Age = 75},
                new FavouriteGivenName() {Id = 7, Name = "Joshua", Age = 93},
                new FavouriteGivenName() {Id = 8, Name = "James", Age = 15},
                new FavouriteGivenName() {Id = 9, Name = "Liam", Age = 73},
                new FavouriteGivenName() {Id = 10, Name = "Max", Age = 63}
            };
            return mostPopular;
        }

        [HttpGet]
        public JsonResult EfficientWebGridPaging(int? page)
        {
            var mostPopular = GetFavouriteGiveNames();
            int skip = page.HasValue ? page.Value - 1 : 0;
            var data = mostPopular.OrderBy(o => o.Id).Skip(skip * 5).Take(5).ToList();
            var grid = new WebGrid(data);
            var htmlString = grid.GetHtml(tableStyle: "webGrid",
                                          headerStyle: "header",
                                          alternatingRowStyle: "alt",
                                          htmlAttributes: new { id = "DataTable" });
            return Json(new
            {
                Data = htmlString.ToHtmlString(),
                Count = mostPopular.Count() / 5
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ContactUs");
            }

            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(ContactUsInput input)
        {
            // Validate the model being submitted
            if (!ModelState.IsValid)
            {
                // If the incoming request is an Ajax Request 
                // then we just return a partial view (snippet) of HTML 
                // instead of the full page
                if (Request.IsAjaxRequest())
                    return PartialView("_ContactUs", input);

                return View(input);
            }

            // TODO: A real app would send some sort of email here

            if (Request.IsAjaxRequest())
            {
                // Same idea as above
                return PartialView("_ThanksForFeedback", input);
            }

            // A standard (non-Ajax) HTTP Post came in
            // set TempData and redirect the user back to the Home page
            TempData["Message"] = string.Format("Thanks for the feedback, {0}! We will contact you shortly.", input.Name);
            return RedirectToAction("Index");
        }

        public ActionResult TestRadio()
        {
            List<aTest> list = new List<aTest>();
            list.Add(new aTest() { ID = 1, Name = "test1" });
            list.Add(new aTest() { ID = 2, Name = "test2" });
            SelectList sl = new SelectList(list, "ID", "Name");

            var model = new RadioModel();
            model.TestRadioList = sl;

            //model.TestRadio = "2";  // Set a default value for the first radio button

            return View(model);
        }

        public ActionResult GetDogListJson()
        {
            List<Models.Dog> dogs = new List<Models.Dog>()
			{
				new Models.Dog {ID = 1, Name = "Mardy", Age = 13, Gender = "Female", Handedness = "None", SpayedNeutered=true, Notes="Beautiful Irish Setter."},
				new Models.Dog {ID = 2, Name = "Izzi", Age = 9, Gender = "Female", Handedness = "Left", SpayedNeutered=true, Notes="Karelian Bear Dog, but not trained for field work."},
				new Models.Dog {ID = 3, Name = "Jewel", Age = 10, Gender = "Female", Handedness = "None", SpayedNeutered=true, Notes="Basenji/Doberman mix with short hair. Why isn't she in Africa where it is warm?"},
				new Models.Dog {ID = 4, Name = "Copper", Age = 3, Gender = "Male", Handedness = "None", SpayedNeutered=true},
				new Models.Dog {ID = 5, Name = "Onyx", Age = 4, Gender = "Female", Handedness = "None", SpayedNeutered=true, Notes="Underweight, suffering from a severe bowel disorder."},
				new Models.Dog {ID = 6, Name = "Raja", Age = 14, Gender = "Female", Handedness = "Right", SpayedNeutered=true, Notes="Older than we first thought, but still loves to run."}
			};
            return Json(dogs, JsonRequestBehavior.AllowGet);
        }
    }
}
