using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    public class SubClassCastInvoker<VM, M>
    {
        internal SubClassCastInvoker()
        {
        }
        internal SubClassCastInvoker(HtmlHelper<VM> htmlHelper, RenderInfo<M> renderInfo)
        {
            this.htmlHelper = htmlHelper;
            this.renderInfo = renderInfo;
        }
        private HtmlHelper<VM> htmlHelper;
        private RenderInfo<M> renderInfo;
        public HtmlHelper<S> To<S>()
            where S:class
        {
            RenderInfo<IDisplayModel> r = htmlHelper.InvokeTransformExt(renderInfo, createHandler<S>(renderInfo.Model), duplicate: true);
            htmlHelper.ViewContext.Writer.WriteLine(r.GetPartialrendering());
            ViewDataDictionary<S> dataDictionary = new ViewDataDictionary<S>(renderInfo.Model as S);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(r.Prefix, "Item");
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            return (new TemplateInvoker<S>().BuildHelper(htmlHelper, dataDictionary));
        }
        public  MvcHtmlString To<S>(out HtmlHelper<S>  newHelper)
           where S : class
        {
            RenderInfo<IDisplayModel> r = htmlHelper.InvokeTransformExt(renderInfo, createHandler<S>(renderInfo.Model), duplicate: true);
            htmlHelper.ViewContext.Writer.WriteLine();
            ViewDataDictionary<S> dataDictionary = new ViewDataDictionary<S>(renderInfo.Model as S);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(r.Prefix, "Item");
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            newHelper = new TemplateInvoker<S>().BuildHelper(htmlHelper, dataDictionary);
            return MvcHtmlString.Create(r.GetPartialrendering());
        }
        public MvcHtmlString Invoke<S>(object template)
            where S : class
        {
            if (template == null) throw(new ArgumentNullException("template"));
            RenderInfo<IDisplayModel> r = htmlHelper.InvokeTransformExt(renderInfo, createHandler<S>(renderInfo.Model), duplicate: true);
            ViewDataDictionary<S> dataDictionary = new ViewDataDictionary<S>(renderInfo.Model as S);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(r.Prefix, "Item");
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            return MvcHtmlString.Create(r.GetPartialrendering()+(new TemplateInvoker<S>().Invoke(htmlHelper, dataDictionary)));
        }
        private IDisplayModel createHandler<S>(object model)
            where S : class
        {
            model = model as S;
            if (model == null) return new SubClassCastTransformer<S>();
            return typeof(SubClassCastTransformer<S>).GetGenericTypeDefinition().MakeGenericType(model.GetType())
                        .GetConstructor(new Type[0]).Invoke(new object[0]) as IDisplayModel;
            
        }
    }
}
