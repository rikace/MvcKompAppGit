using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKompApp.ViewModels;

namespace MvcKompApp.Controllers
{
    public class PersonController : Controller
    {
        public ActionResult Index()
        {
            var model = new PersonViewModel
                            {
                                Id = 12345,
                                IsCompany = true,
                                Reliable = null,
                                Name = "e-tennis.NET",
                                Notes = "My company",
                                Foundation = new DateTime(2005, 3, 12),
                                Website = "http://www.e-tennis.net",
                                Contact = new ContactInfo
                                {
                                    Email = "d@d.com",
                                    FullName = "D.E.",
                                    PhoneNumber = "000"
                                }
                            };
            return View(model);
        }

        public ActionResult Edit()
        {
            var model = new PersonViewModel
            {
                Id = 12345,
                IsCompany = true,
                Reliable = null,
                Name = "e-tennis.NET",
                Notes = "My company",
                Foundation = new DateTime(2005, 3, 12),
                Website = "http://www.e-tennis.net",
                Contact = new ContactInfo
                {
                    Email = "d@d.com",
                    FullName = "D.E.",
                    PhoneNumber = "000"
                }
            };
            return View(model);
        }
    }
}