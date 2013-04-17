using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKompApp.ViewModels;

namespace MvcKompApp.Controllers
{
    public class ProductController : Controller
    {
        static List<ProductViewModel> _prodLst = ProductList.init();

        public ActionResult Index()
        {
            return View(_prodLst);
        }


        public ViewResult Details(string id)
        {
            return View(GetProduct(id));
        }

        public ActionResult Edit(string id)
        {
            return View(GetProduct(id));
        }


        [HttpPost]
        public ActionResult Edit(ProductViewModel um)
        {
            if (!TryUpdateModel(um))
                return View(um);

            // ToDo: add persistent to DB.
            ProductList.Update(um);
            return View("Details", um);
        }

        public ActionResult About()
        {
            return View();
        }


        ProductViewModel GetProduct(string prodName)
        {
            ProductViewModel prodMdl = null;

            foreach (ProductViewModel pvm in _prodLst)
                if (pvm.Name == prodName)
                    prodMdl = pvm;

            return prodMdl;
        }
    }
}