using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKompApp.Models;

namespace MvcKompApp.Controllers
{
    public class TaskController : Controller
    {
        private TaskDBContext db = new TaskDBContext();

        //
        // GET: /Task/

        public ViewResult Index()
        {
            return View(db.Tasks.ToList());
        }

        //
        // GET: /Task/Details/5

        public ViewResult Details(int id)
        {
            TaskItem taskitem = db.Tasks.Find(id);
            return View(taskitem);
        }

        //
        // GET: /Task/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Task/Create

        [HttpPost]
        public ActionResult Create(TaskItem taskitem)
        {
            if (ModelState.IsValid)
            {
                taskitem.Completed = false;
                taskitem.EntryDate = DateTime.Now;
                db.Tasks.Add(taskitem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(taskitem);
        }

        //
        // GET: /Task/Edit/5

        public ActionResult Edit(int id)
        {
            TaskItem taskitem = db.Tasks.Find(id);
            return View(taskitem);
        }

        //
        // POST: /Task/Edit/5

        [HttpPost]
        public ActionResult Edit(TaskItem taskitem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taskitem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(taskitem);
        }

        //
        // GET: /Task/Delete/5

        public ActionResult Delete(int id)
        {
            TaskItem taskitem = db.Tasks.Find(id);
            return View(taskitem);
        }

        //
        // POST: /Task/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            TaskItem taskitem = db.Tasks.Find(id);
            if (taskitem == null)
                return HttpNotFound();

            db.Tasks.Remove(taskitem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public string SearchTask(FormCollection fc, string task)
        {
            return "<h3>From [HttpPost] " + task + "</h3>";
        }

        public ActionResult SearchTask(string task, string iscomplete)
        {
            var tasks = from t in db.Tasks
                        select t;

            bool iscompleteBool;
            if (bool.TryParse(iscomplete, out iscompleteBool))
            {
                tasks = tasks.Where(t => t.Completed == iscompleteBool);
            }

            if (!string.IsNullOrEmpty(task))
            {
                tasks = tasks.Where(t => t.Task.Contains(task));
            }
            return View(tasks);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}