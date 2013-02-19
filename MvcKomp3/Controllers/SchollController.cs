using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.Models;

namespace MvcKomp3.Controllers
{   
    public class SchollController : Controller
    {
        private SchoolContext context = new SchoolContext();

        //
        // GET: /Scholl/

        public ViewResult Index()
        {
            return View(context.Courses.Include(course => course.Department).Include(course => course.Enrollments).Include(course => course.Instructors).ToList());
        }

        //
        // GET: /Scholl/Details/5

        public ViewResult Details(int id)
        {
            Course course = context.Courses.Single(x => x.CourseID == id);
            return View(course);
        }

        //
        // GET: /Scholl/Create

        public ActionResult Create()
        {
            ViewBag.PossibleDepartments = context.Departments;
            return View();
        } 

        //
        // POST: /Scholl/Create

        [HttpPost]
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                context.Courses.Add(course);
                context.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.PossibleDepartments = context.Departments;
            return View(course);
        }
        
        //
        // GET: /Scholl/Edit/5
 
        public ActionResult Edit(int id)
        {
            Course course = context.Courses.Single(x => x.CourseID == id);
            ViewBag.PossibleDepartments = context.Departments;
            return View(course);
        }

        //
        // POST: /Scholl/Edit/5

        [HttpPost]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                context.Entry(course).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleDepartments = context.Departments;
            return View(course);
        }

        //
        // GET: /Scholl/Delete/5
 
        public ActionResult Delete(int id)
        {
            Course course = context.Courses.Single(x => x.CourseID == id);
            return View(course);
        }

        //
        // POST: /Scholl/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = context.Courses.Single(x => x.CourseID == id);
            context.Courses.Remove(course);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}