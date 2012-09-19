using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCControlsToolkit.Core
{
    public interface ITemplateInvoker
    {
        string Invoke<M>(HtmlHelper<M> fatherHelper, ViewDataDictionary viewDictionary);
        string Invoke<M>(HtmlHelper<M> fatherHelper, object model, string prefix, string truePrefix=null);
        HtmlHelper BuildHelper(HtmlHelper fatherHelper, object model, string prefix, bool useContextWriter);
    }
}
