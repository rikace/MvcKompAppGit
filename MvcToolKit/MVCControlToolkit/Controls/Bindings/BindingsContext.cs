using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controls;
using System.IO;

namespace MVCControlsToolkit.Controls.Bindings
{
    public class BindingsContext<VM, T>: IDisposable
        where T : class, new()
    {
        private string prefix;
        IBindingsBuilder<T> bindings;
        HtmlHelper<VM> htmlHelper;
        private TextWriter writer;
        public BindingsContext(IBindingsBuilder<T> bindings, string prefix, HtmlHelper<VM> htmlHelper, TextWriter writer)
        {
            this.bindings= bindings;
            this.prefix=prefix;
            this.htmlHelper=htmlHelper;
            this.writer=writer;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);   
        }
        bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            if (!_disposed)
            {
                if (disposing)
                {
                    string res = new KoAutomaticBinder<T>((htmlHelper.ViewContext.Writer as StringWriter).ToString(),
                        bindings,
                        prefix).ToString();
                    writer.Write(res);
                    htmlHelper.ViewContext.Writer = writer;
                    htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = prefix;
                    htmlHelper.ViewData.Remove("ClientBindings");
                }
                _disposed = true;
            }
        }
    }
}
