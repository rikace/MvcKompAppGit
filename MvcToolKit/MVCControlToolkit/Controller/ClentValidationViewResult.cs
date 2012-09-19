using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MVCControlsToolkit.Core;


namespace MVCControlsToolkit.Controller
{

    public static class ClentValidationExtensions
    {
        public static ClentValidationViewResult ClentValidationView(this System.Web.Mvc.Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            return new ClentValidationViewResult(viewName, model)
            {
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }
        public static ClentValidationViewResult ClentValidationView(this System.Web.Mvc.Controller controller, object model)
        {
            return ClentValidationView(controller, null, model);
        }
        public static ClentValidationViewResult ClentValidationView(this System.Web.Mvc.Controller controller, string viewName)
        {
            return ClentValidationView(controller, viewName, null);
        }
    }
    public class ClentValidationViewResult : ViewResult
    {
        const string DummyFormId = "DummyFormId";
        bool oldClientSetting;
        object model;
        public ClentValidationViewResult(string viewName, object model)
        {
            ViewName = viewName;
            this.model = model;
            
        }
        public ClentValidationViewResult(object model)
        {
            ViewName = null;
            this.model = model;

        }
        public ClentValidationViewResult(string viewName=null)
        {
            ViewName = viewName;
            this.model = null;

        }
 
        public override void ExecuteResult(ControllerContext context)
        {
            context.Controller.ViewData.Model = model;
            ViewData = context.Controller.ViewData;
            TempData = context.Controller.TempData;
            var result = base.FindView(context);
            var viewContext = new ViewContext(context, result.View, ViewData, TempData, context.HttpContext.Response.Output);
 
            BeginCapturingValidation(viewContext);
            base.ExecuteResult(context);
            EndCapturingValidation(viewContext);
 
            result.ViewEngine.ReleaseView(context, result.View);
        }
 
        private void BeginCapturingValidation(ViewContext viewContext)
        {

            oldClientSetting = viewContext.ClientValidationEnabled;
            viewContext.ClientValidationEnabled = true;
            viewContext.FormContext = new FormContext { FormId = DummyFormId };
        }
 
        private void EndCapturingValidation(ViewContext viewContext)
        {

            switch (MvcEnvironment.Validation(viewContext))
            {
                case ValidationType.StandardClient:
                    viewContext.OutputClientValidation();
                    break;
                case ValidationType.UnobtrusiveClient: break;
                default: break;
 
            }
            viewContext.ClientValidationEnabled = oldClientSetting;
            
        }
    }
}
