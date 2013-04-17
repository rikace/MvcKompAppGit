using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MvcKompApp.Models;

namespace MvcKompApp.Controllers
{
    public class TaskController : Controller //AsyncController
    {
        private TaskDBContext db = new TaskDBContext();

        //public void IndexAsync()
        //{
        //    AsyncManager.OutstandingOperations.Completed += OutstandingOperations_Completed;
        //    AsyncManager.OutstandingOperations.Increment(1);

        //    Task.Factory.StartNew(() =>
        //    {
        //        var tasks = db.Tasks.ToList();
        //        AsyncManager.Parameters["tasks"] = tasks;
        //        AsyncManager.OutstandingOperations.Decrement(1);
        //    });
        //}

        //public ViewResult IndexCompleted(IList<TaskItem> tasks)
        //{
        //    ViewBag.TaskList = tasks.Select(t => new { id = t.Id, Value = t.Task }); 
        //    return View(tasks);
        //}

        public ViewResult Index()
        {
            var tasks = db.Tasks.ToList();
            ViewBag.TaskList = tasks.Select(s=>s.Task);
            return View(tasks);

        }



        void OutstandingOperations_Completed(object sender, EventArgs e)
        {
            Console.WriteLine("Completed - {0}", sender.GetType().Name);
        }

        public ActionResult GetTasksDDL(string value)
        {
            return Json(new object[] { "Riccardo", "Bugghina" }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetTasksDDL(string value)
        //{
        //    return Json(new object[] { "Riccardo", "Bugghina" }, JsonRequestBehavior.AllowGet);
        //}

        //
        // GET: /Task/

        //public ViewResult Index()
        //{
        //    return View(db.Tasks.ToList());
        //}

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
        [ActionName("Edit")]
        public ActionResult EditPost(int Id)// TaskItem taskitem)
        {
            var taskitem = db.Tasks.First(t => t.Id == Id);
            if (ModelState.IsValid)
            {

                bool isVupdated = TryUpdateModel(taskitem);
                //db.Entry(taskitem).State = EntityState.Modified;
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