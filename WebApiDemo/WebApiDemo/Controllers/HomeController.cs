using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Controllers;

namespace WebApiDemo.Controllers
{
    [WebApiDemo.Filters.HelperFilter]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("drills");
        }

        public ActionResult Docs()
        {
            var apiExlorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();
            var apiInfo = apiExlorer.ApiDescriptions.ToLookup(x => x.ActionDescriptor.ControllerDescriptor.ControllerName);
            return View(apiInfo);
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult ContactsGrid()
        {
            var contactsController = new CustomerGridController();
            var contacts = contactsController.Get();
            return PartialView("ContactsGrid", contacts);
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult GetContactsById(int id)
        {
            var contactsController = new CustomerGridController();
            var contacts = contactsController.Get(id);
            return PartialView("EditContact", contacts);
        }
    }
}
