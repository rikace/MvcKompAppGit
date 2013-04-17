using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKompApp.Framework;

namespace MvcKompApp.Controllers
{
    public class PdfController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Create your PDF document";
            return View();
        }

        [HttpGet]
        [ActionName("Pdf")]
        public ActionResult PreparePdf(String author)
        {
            ViewBag.Author = "Riccardo Terrell";
            return View();
        }

        [HttpPost]
        [ActionName("Pdf")]
        public ActionResult CreatePdf(String author)
        {
            var documentName = Server.MapPath("/uploads/Sample.pdf");
            var templateName = Server.MapPath("/uploads/Sample.dotx");

            WordDocument.CreateSampleWordDocument(documentName, templateName, DateTime.Now, author);
            return File(documentName, "application/pdf");
        }
    }
}