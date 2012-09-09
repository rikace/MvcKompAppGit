using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcKompApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult PartialViewRefreshHandler()
        {
            return View();
        }

        public ActionResult RefreshPartial(FormCollection form)
        {
            string name = form["Name"];

            ViewBag.Name = name == null ? "No one, just refreshing" : name;
            ViewBag.DateTime = DateTime.Now.ToString();

            return PartialView("_DateTimePartial");
        }
    }
}
