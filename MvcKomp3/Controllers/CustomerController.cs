using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKompApp.Infrastructure;
using MvcKompApp.WorkerServices.Abstraction;
using MvcKompApp.WorkerServices.Customer;
using MvcKompMvc4.Infrastructure;

namespace MvcKompApp.Controllers
{
    [Mvc3Filter.Filters.TraceActionFilter]   // trace all action methods in this controller
    public class CustomerController : Controller
    {
        private readonly ICustomerWorkerServices _service;

        public CustomerController(ICustomerWorkerServices service)
        {
            _service = service;
        }

        public CustomerController()  // TODO DI ninjeckt
            : this(new CustomerWorkerServices())
        { }

        public ActionResult Index()
        {
            var customers = _service.FindAllCustomers();
            ViewBag.Country = GetCountrySelectList();

            if (TempData.ContainsKey("YouSelected"))
                ViewBag.YouSelected = TempData["YouSelected"];

            return View(customers.Customers);
        }

        [IsAjax(false)]
        public ActionResult Edit(int id)
        {
            var custmer = _service.FindCustmer(id);

            if (custmer != null)
            {
                if (Request.IsAjaxRequest())
                {
                    return View("_customerEdit", custmer);
                }
                return View(custmer);
            }
            return View();
        }


        [IsAjaxAttribute(true), ActionName("Edit")]
        public ActionResult Edit_Ajax(int id)
        {
            var custmer = _service.FindCustmer(id);

            if (custmer != null)
            {
                if (Request.IsAjaxRequest())
                {
                    return View("_customerEdit", custmer);
                }
                return View(custmer);
            }
            return View();
        }

        [AjaxOnly, ActionName("EditAjax")]
        public ActionResult Update([Bind(Prefix = "customerList")] String Id)
        {
            var custmerFInd = _service.FindCustmer(Convert.ToInt32(Id));
            return PartialView("_customerEdit", custmerFInd);
        }

        public ActionResult UpdateCustmer(MvcKompApp.Models.Customer customer)
        {
            var custmerToUpdate = _service.FindCustmer(customer.Id);
            if (customer != null)
            {
                TryValidateModel(customer);
                //update cutmer
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult IndexDDL(string Countries, string States)
        {
            ViewBag.Countries = GetCountrySelectList();

            int stateID = -1;
            if (!int.TryParse(States, out stateID))
            {
                ViewBag.YouSelected = "You must select a Country and State";
                return View();
            }

            var state = from s in MvcKompApp.Models.State.GetStates()
                        where s.StateID == stateID
                        select s.StateName;

            var country = from s in MvcKompApp.Models.Country.GetCountries()
                          where s.Code == Countries
                          select s.Name;


            TempData["YouSelected"] = "You selected " + country.SingleOrDefault()
                                 + " And " + state.SingleOrDefault();
            //ViewBag.YouSelected = "You selected " + country.SingleOrDefault()
            //                     + " And " + state.SingleOrDefault();
            return  RedirectToAction("Index");
        }

        public ActionResult StateList(string ID)
        {
            string Code = ID;
            var states = from s in MvcKompApp.Models.State.GetStates()
                         where s.Code == Code
                         select s;

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(
                                states.ToArray(),
                                "StateID", "StateName")
                           , JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }

        private SelectList GetCountrySelectList()
        {
            var countries = MvcKompApp.Models.Country.GetCountries();
            return new SelectList(countries.ToArray(), "Code", "Name");
        }
    }
}
