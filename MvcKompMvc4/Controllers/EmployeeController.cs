using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.UI;
using Entities;
using MvcKompApp.Infrastructure;
using MvcKompApp.Models;

namespace MvcKompApp.Controllers
{
    //[Authorize]
    public class EmployeeController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();

        public int PageSize = 4;
        
        [OutputCache(CacheProfile = "Aggressive", VaryByParam = "parameter", Location = OutputCacheLocation.Server)]
        //[OutputCache(Duration = 30, VaryByParam = "parameter", Location = OutputCacheLocation.Server)]
        // VaryByParam = none -> always cahche the same content
        // VaryByParam = * -> cache every permutation
        // VaryByParam = ~name parameter~ cache every value of name parameter
        public ActionResult BeginFormTest(string parameter)
        {
            ViewData["Message"] = parameter ?? "";

            DateTime date = DateTime.Now;

            return View(date);
        }
        //
        // GET: /Employee/

        public ActionResult Index(string q = null, int page = 1)
        {
            List<Employee> employees = null;
            if (HttpContext.Cache["Employees"] == null)
            {
                employees = db.Employees.Include("Employee1").ToList();
                HttpContext.Cache.Insert("Employees", employees, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10));
            }
            else
            {
                employees = HttpContext.Cache["Employees"] as List<Employee>;
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Somethng", employees);
            }

            if (q != null)
            {
                employees = employees.Where(e => e.FirstName.Contains(q)).ToList();
            }

            EmployeeWithPaging viewModel = new EmployeeWithPaging
            {
                Employees = employees.Skip((page - 1) * PageSize)
                    .Take(PageSize).ToList(),
                Paging = new PagingInfo
              {
                  CurrentPage = page,
                  ItemsPerPage = PageSize,
                  TotalItems = employees.Count
              }
            };

            return View(viewModel);
        }

        //
        // GET: /Employee/Details/5

        public ViewResult Details(int id)
        {
            Employee employee = db.Employees.Single(e => e.EmployeeID == id);
            return View(employee);
        }

        //
        // GET: /Employee/Create

        public ActionResult Create()
        {
            ViewBag.ReportsTo = new SelectList(db.Employees, "EmployeeID", "LastName");

          

            return View();
        }

        //
        // POST: /Employee/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Employees.AddObject(employee);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var propertyError in entityError.ValidationErrors)
                    {
                        ModelState.AddModelError(propertyError.PropertyName, propertyError.ErrorMessage);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            ViewBag.ReportsTo = new SelectList(db.Employees, "EmployeeID", "LastName", employee.ReportsTo);
            return View(employee);
        }

        //
        // GET: /Employee/Edit/5

        public ActionResult Edit(int id)
        {
            Employee employee = db.Employees.Single(e => e.EmployeeID == id);
            ViewBag.ReportsTo = new SelectList(db.Employees, "EmployeeID", "LastName", employee.ReportsTo);
            return View(employee);
        }

        //
        // POST: /Employee/Edit/5

        [HttpPost]
        public ActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Attach(employee);
                db.ObjectStateManager.ChangeObjectState(employee, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReportsTo = new SelectList(db.Employees, "EmployeeID", "LastName", employee.ReportsTo);
            return View(employee);
        }

        //
        // GET: /Employee/Delete/5

        public ActionResult Delete(int id)
        {
            Employee employee = db.Employees.Single(e => e.EmployeeID == id);
            return View(employee);
        }

        //
        // POST: /Employee/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Single(e => e.EmployeeID == id);
            db.Employees.DeleteObject(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult Time()
        {
            return PartialView(DateTime.Now);
        }

        public string SearchEmployee(string q, int limit)
        {
            var employee = db.Employees.Where(e => e.FirstName.Contains(q))
                .OrderByDescending(e => e.HireDate)
                .Take(limit)
                .Select(e => e.FirstName);

            return string.Join(Environment.NewLine, employee.ToArray());
        }

        public ActionResult SearchEmployee2(string q)
        {
            var employee = db.Employees.Where(e => e.FirstName.Contains(q))
                .OrderByDescending(e => e.HireDate)
               
                .Select(e => e.FirstName);

            return Json(employee.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public string QuickSearch(string q)
        {
            return "dddd";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}