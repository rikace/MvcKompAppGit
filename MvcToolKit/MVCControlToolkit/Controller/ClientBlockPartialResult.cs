/* ****************************************************************************
 *
 * Copyright (c) Francesco Abbruzzese. All rights reserved.
 * francesco@dotnet-programming.com
 * http://www.dotnet-programming.com/
 * 
 * This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
 * and included in the license.txt file of this distribution.
 * 
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controls;
using MVCControlsToolkit.Controls.Bindings;
using System.IO;

namespace MVCControlsToolkit.Controller
{
    public class ClientBlockPartialResult : PartialViewResult, IViewDataContainer
    {
         object model;
        string modelJsName;
        string blockId;
        public ClientBlockPartialResult(string viewName, object model, string modelJsName, string blockId)
        {
            ViewName = viewName;
            this.model = model;
            this.modelJsName = modelJsName;
            this.blockId = blockId;
            
        }
        public ClientBlockPartialResult(object model, string modelJsName, string blockId)
        {
            ViewName = null;
            this.model = model;
            this.modelJsName = modelJsName;
            this.blockId = blockId;
        }

        public Func<string, string> PrepareForCompile<T>(ViewContext vc)
            where T: class, new()
        {
            HtmlHelper<T> htmlHelper = new HtmlHelper<T>(vc, this);
            IBindingsBuilder<T> bindings = htmlHelper.ClientViewModel(modelJsName, m => m, blockId, false);
            string viewInitialization = (vc.Writer as StringWriter).ToString();
            ViewData["ClientBindings"] = bindings;
            ViewData["_TruePrefix_"] = string.Empty;
            ViewData.TemplateInfo.HtmlFieldPrefix = ClientTemplateHelper.templateSymbol + ".A";
                Func<string, string> res = (input) => {
                    return new KoAutomaticBinder<T>(input, bindings, string.Empty, viewInitialization).ToString();
                };
                return res;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (model == null) throw (new ArgumentNullException("model"));
            if (string.IsNullOrWhiteSpace(modelJsName)) throw (new ArgumentNullException("modelJsName"));
            ViewData = context.Controller.ViewData ;
            ViewData.Model = model;
            TempData = context.Controller.TempData;

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (String.IsNullOrEmpty(ViewName))
            {
                ViewName = context.RouteData.GetRequiredString("action");
            }

            ViewEngineResult result = null;

            if (View == null)
            {
                result = FindView(context);
                View = result.View;
            }

            StringWriter writer = new System.IO.StringWriter();
            ViewContext viewContext = new ViewContext(context, View, ViewData, TempData, writer);
            Func<string, string> compiler = typeof(ClientBlockPartialResult).GetMethod("PrepareForCompile")
                .MakeGenericMethod(model.GetType())
                .Invoke(this, new object[] {viewContext}) as Func<string, string>;
            writer = new System.IO.StringWriter();
            viewContext = new ViewContext(context, View, ViewData, TempData, writer);

            TextWriter writer1 = context.HttpContext.Response.Output;
            View.Render(viewContext, writer);
            context.HttpContext.Response.Output.Write(compiler(writer.ToString()));
            
            if (result != null)
            {
                result.ViewEngine.ReleaseView(context, View);
            }
   
        }
    }
}
