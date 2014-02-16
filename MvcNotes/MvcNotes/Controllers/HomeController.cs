using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;

namespace MvcNotes.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class HomeController : Controller
    {
        [HttpGet]
        public JsonResult SayHelloMessage(int id)
        {
            if (Request.IsAjaxRequest())
            {
                // Display the confirmation message
                var MessageObject = new LogMessage
                {
                    Message = (id.ToString() + " Message receied from JSON Client and sending back with this text")
                };

                // Remove JsonRequestBehavior.AllowGet and see.
                return Json(MessageObject, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpPost]
        [ActionName("pdf")]
        public ActionResult CreatePdf(String author)
        {
            var documentName = Server.MapPath("/Sample.pdf");
         return File(documentName, "application/pdf");
        }


        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        [OutputCache(Duration = 30, Location = OutputCacheLocation.Downstream, VaryByParam = "none")]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    [Serializable]
    public class LogMessage
    {
        public string Message { get; set; }
    }
}
