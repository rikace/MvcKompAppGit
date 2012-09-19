using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace MVCControlsToolkit.Core
{
    public class TemplateInvoker<T>:IViewDataContainer, ITemplateInvoker
    {
        private string fileTemplate;
        private Func<HtmlHelper<T>, string> functionTemplate;
        private Func<HtmlHelper<T>, HelperResult> helperTemplate;
        private MVCControlsToolkit.Controls.Bindings.IBindingsBuilder<T> bindings = null;
        public TemplateInvoker(object template)
        {
            helperTemplate = template as Func<HtmlHelper<T>, HelperResult>;
            functionTemplate = template as Func<HtmlHelper<T>, string>;
            fileTemplate = template as string;
            if (functionTemplate == null && fileTemplate == null && helperTemplate==null)
                throw (new ArgumentException(Resources.templateNotValid));
        }
        public TemplateInvoker(object template, MVCControlsToolkit.Controls.Bindings.IBindingsBuilder<T> bindings)
        {
            helperTemplate = template as Func<HtmlHelper<T>, HelperResult>;
            functionTemplate = template as Func<HtmlHelper<T>, string>;
            fileTemplate = template as string;
            if (functionTemplate == null && fileTemplate == null && helperTemplate == null)
                throw (new ArgumentException(Resources.templateNotValid));
            this.bindings = bindings;
        }
        public TemplateInvoker()
        {
        }
        internal static Type ExtractModelType(object template)
        {
            
            string fileTemplate = template as string;
            if (fileTemplate == null)
            {
                Type[] res = template.GetType().GetGenericArguments();
                if (res.Length == 0)
                {
                    throw (new ArgumentException(Resources.NotIstantiatedType));
                }
                else
                {
                    res=res[0].GetGenericArguments();
                    if (res.Length == 0 || res[0].ContainsGenericParameters)
                        throw (new ArgumentException(Resources.NotIstantiatedType));
                    return res[0];
                }
            }
            
            else
            {
                throw (new ArgumentException(Resources.TypeSafeTemplateNotValid));
            }
        }
        public HtmlHelper<T> BuildHelper(HtmlHelper fatherHelper, ViewDataDictionary viewDictionary, bool useContextWriter=false)
        {
            ControllerContext controllerContext = new ControllerContext
                    (
                        fatherHelper.ViewContext.HttpContext,
                        fatherHelper.ViewContext.RouteData,
                        fatherHelper.ViewContext.Controller
                    );
            ViewContext viewContext = new ViewContext(
                controllerContext,
                fatherHelper.ViewContext.View,
                viewDictionary,
                fatherHelper.ViewContext.TempData,
                useContextWriter ? (contextWriter=new System.IO.StringWriter()) : fatherHelper.ViewContext.Writer

                    );
            viewContext.ClientValidationEnabled = fatherHelper.ViewContext.ClientValidationEnabled;
            viewContext.FormContext = fatherHelper.ViewContext.FormContext;
            this.ViewData = viewDictionary;
            if (bindings != null) viewDictionary["ClientBindings"] = bindings;
                return new HtmlHelper<T>(
                viewContext,
                this
                );
        }
        public HtmlHelper BuildHelper(HtmlHelper fatherHelper, object model, string prefix, bool useContextWriter = false)
        {
            T tModel = default(T);
            if (model != null) tModel = (T)model;
            ViewDataDictionary<T> dataDictionary = new ViewDataDictionary<T>(tModel);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = prefix;
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, fatherHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            return BuildHelper(fatherHelper, dataDictionary, useContextWriter);
        }
        public string  Invoke<M>(HtmlHelper<M> fatherHelper, ViewDataDictionary viewDictionary)
        {
            
            if (fileTemplate != null)
            {
                if (bindings != null) viewDictionary["ClientBindings"] = bindings;
                string res = fatherHelper.Partial(fileTemplate, viewDictionary).ToString();
                return res;
            }
            else if (helperTemplate != null)
            {
                string res = helperTemplate(BuildHelper(fatherHelper, viewDictionary, true)).ToString();
                contextWriter.Write(res);
                return contextWriter.ToString();
            }
            else if (functionTemplate != null)
            {
                string res = functionTemplate(BuildHelper(fatherHelper, viewDictionary, true));
                contextWriter.Write(res);
                return contextWriter.ToString();
            }
            return string.Empty;
        }
        private string InnerInvoke(HtmlHelper fatherHelper, ViewDataDictionary viewDictionary)
        {
            
            if (fileTemplate != null)
            {
                if (bindings != null) viewDictionary["ClientBindings"] = bindings;
                string res = fatherHelper.Partial(fileTemplate, viewDictionary).ToString();
                return res;
            }
            else if (helperTemplate != null)
            {
                string res = helperTemplate(BuildHelper(fatherHelper, viewDictionary, true)).ToString();
                contextWriter.Write(res);
                return contextWriter.ToString();
            }
            else if (functionTemplate != null)
            {
                string res = functionTemplate(BuildHelper(fatherHelper, viewDictionary, true));
                contextWriter.Write(res);
                return contextWriter.ToString();
            }
            return string.Empty;
        }
        public string Invoke<M>(HtmlHelper<M> fatherHelper,T model, string prefix, string truePrefix = null)
        {
            ViewDataDictionary<T> dataDictionary = new ViewDataDictionary<T>(model);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = prefix;
            if (truePrefix != null) dataDictionary["_TruePrefix_"] = truePrefix;
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, fatherHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);

            return Invoke(fatherHelper, dataDictionary);
        }
        public string InvokeNM(HtmlHelper fatherHelper, ViewDataDictionary viewDictionary)
        {

            if (fileTemplate != null)
            {
                if (bindings != null) viewDictionary["ClientBindings"] = bindings;
                string res = fatherHelper.Partial(fileTemplate, viewDictionary).ToString();
                return res;
            }
            else if (functionTemplate != null)
            {
                string res = functionTemplate(BuildHelper(fatherHelper, viewDictionary, true));
                contextWriter.Write(res);
                return contextWriter.ToString();
            }
            return string.Empty;
        }
        public string InvokeNM(HtmlHelper fatherHelper, T model, string prefix)
        {
            ViewDataDictionary<T> dataDictionary = new ViewDataDictionary<T>(model);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = prefix;
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, fatherHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);

            return InvokeNM(fatherHelper, dataDictionary);
        }
        public string InvokeVirtual(HtmlHelper fatherHelper, string prefix)
        {
            T model = default(T);
            Type basicType = typeof(T);
            if (basicType.IsClass)
            {
                var constructor = basicType.GetConstructor(new Type[0]);
                if (constructor != null) model = (T)constructor.Invoke(new object[0]);
            }
            ViewDataDictionary<T> dataDictionary = new ViewDataDictionary<T>(model);
            dataDictionary["_TemplateLevel_"] = 0;
            dataDictionary.TemplateInfo.HtmlFieldPrefix = prefix;
            return InnerInvoke(fatherHelper, dataDictionary);
        }
        public string Invoke<M>(HtmlHelper<M> fatherHelper, object model, string prefix, string truePrefix=null)
        {
            return Invoke<M>(fatherHelper, (T)model, prefix, truePrefix);
        }

        private System.IO.StringWriter contextWriter;
        ViewDataDictionary _ViewData;
        public ViewDataDictionary ViewData
        {
            get
            {
                return _ViewData;
            }
            set
            {
                _ViewData=value;
            }
        }
    }
}
