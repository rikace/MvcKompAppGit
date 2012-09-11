using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcKompApp.Framework
{
    public class PdfResult : ActionResult
    {
        private readonly String _fileName;
        public PdfResult(String fileName)
        {
            _fileName = fileName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.WriteFile(_fileName);
            return;
        }
    }
}