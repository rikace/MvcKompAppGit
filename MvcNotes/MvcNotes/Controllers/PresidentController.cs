using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcNotes.Controllers
{
    public class PresidentController : Controller
    {
        //
        // GET: /President/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /President/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /President/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /President/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /President/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /President/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /President/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /President/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
