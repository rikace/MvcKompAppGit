using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MvcKompApp.Models;

namespace MvcKompApp.Controllers
{
    public class NavController : Controller
    {
        private IProductRepository repository;

        public NavController(IProductRepository repo)
        {
            repository = repo;
        }

        [ChildActionOnly]
        public string MyAction()
        {
            //return View(new[] { "Test Action 1", "Test Action 2" });
            return "Test from action 1";
        }

        public ViewResult Menu(string category = null)
        {

            ViewBag.SelectedCategory = category;

            IEnumerable<string> categories = repository.Products
                                    .Select(x => x.Category.CategoryName)
                                    .Distinct()
                                    .OrderBy(x => x);

            return View(categories);
        }
    }
}