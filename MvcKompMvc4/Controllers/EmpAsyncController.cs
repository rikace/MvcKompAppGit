using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Entities;

namespace MvcKompApp.Controllers
{
    public class EmpAsyncController : AsyncController
    {
        public void IndexAsync()
        {
            AsyncManager.OutstandingOperations.Increment();

            using (NorthwindEntities db = new NorthwindEntities())
            {
                AsyncManager.Parameters["Emps"] = db.Employees.ToList();
            }
            
            AsyncManager.OutstandingOperations.Decrement();
        }

        public ActionResult IndexCompleted(List<Employee> emps)
        {
            return View(emps);
        }
    }
}
