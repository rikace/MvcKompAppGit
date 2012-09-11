using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKompApp.WorkerServices;

namespace MvcKompApp.Controllers
{
    public class VisitorSiteController : Controller
    {
        private readonly IVisitorSiteService _service;

        public VisitorSiteController()          // TODO use DI ninjeck
            : this(new VisitorSiteService())
        { }

        public VisitorSiteController(IVisitorSiteService service)
        {
            _service = service;
        }

        //
        // GET: /VisitorSite/  
        public ActionResult Index()
        {
            var visitors = _service.GetVisitors();
            return View(visitors);
        }

    }
}
