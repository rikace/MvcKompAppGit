using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKompApp.Infrastructure;
using MvcKompApp.WorkerServices.Abstraction;
using MvcKompApp.WorkerServices.Customer;

namespace MvcKompApp.Controllers
{
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
            return View(customers.Customers);
        }

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

    }
}
