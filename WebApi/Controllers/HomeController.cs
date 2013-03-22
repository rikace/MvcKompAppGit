using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ContactsGrid()
        {
            var contactsController = new CustomerGridController();
            var contacts = contactsController.Get();
            return PartialView("ContactsGrid", contacts);
        }

        [HttpGet]
        public ActionResult GetContactsById(int id)
        {
            var contactsController = new CustomerGridController();
            var contacts = contactsController.Get(id);
            return PartialView("EditContact", contacts);
        }
    }
}
