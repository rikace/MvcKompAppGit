using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKompApp.Models;

namespace MvcKompApp.Controllers
{
    public class AppointmentController : Controller
    {
        //private IAppointmentRepository repository;

        //public AppointmentController(IAppointmentRepository repo)
        //{
        //    repository = repo;
        //}

        public ViewResult MakeBooking()
        {
            return View(new Appointment { Date = DateTime.Now });
        }

        [ValidateInput(false)]
        [HttpPost]
        public ViewResult MakeBooking(Appointment appt)
        {

            //if (string.IsNullOrEmpty(appt.ClientName)) {
            //    ModelState.AddModelError("ClientName", "Please enter your name");
            //}

            //if (ModelState.IsValidField("Date") && DateTime.Now > appt.Date) {
            //    ModelState.AddModelError("Date", "Please enter a date in the future");
            //}

            //if (!appt.TermsAccepted) {
            //    ModelState.AddModelError("TermsAccepted", "You must accept the terms");
            //}

            //if (ModelState.IsValidField("ClientName") && ModelState.IsValidField("Date")
            //    && appt.ClientName == "Joe" && appt.Date.DayOfWeek == DayOfWeek.Monday) {
            //    ModelState.AddModelError("", "Joe cannot book appointments on Mondays");
            //}

            if (ModelState.IsValid)
            {
                // repository.SaveAppointment(appt);
                return View("Completed", appt);
            }
            else
            {
                return View();
            }
        }

        // Use for remote Validation
        public JsonResult ValidateDate(string Date)
        {
            DateTime parsedDate;
            if (!DateTime.TryParse(Date, out parsedDate))
            {
                return Json("Please enter a valid date (mm/dd/yyyy)",
                    JsonRequestBehavior.AllowGet);
            }
            else if (DateTime.Now > parsedDate)
            {
                return Json("Please enter a date in the future", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ValidateClientName(string ClientName)
        {
            if (string.IsNullOrWhiteSpace(ClientName) || ClientName.ToLower() != "ricky")
                return Json(false, JsonRequestBehavior.AllowGet);

            return Json(true, JsonRequestBehavior.AllowGet);
        
        }
    }
}