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
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Globalization;
namespace MVCControlsToolkit.Core
{
    internal class TransformationContext
    {
        internal static TransformationContext Instance;
        protected class TransformerInfos
        {
            public int Code;
            public int ContextCode;
        }
        protected int CurrContext;
        protected IDictionary<Type, TransformerInfos> TransformersDirectory = new Dictionary<Type, TransformerInfos>();
        private StringBuilder ToRender;
        private bool ContextOn=false;
        internal static void NewContext()
        {
            if (!HttpContext.Current.Items.Contains(ExDefaultBinder.TypeRegisterPrefix))
            {
                HttpContext.Current.Items[ExDefaultBinder.TypeRegisterPrefix] = new object();
                Instance = new TransformationContext();
            }
            Instance.CurrContext++;
            Instance.ToRender= new StringBuilder();
            Instance.ContextOn = true;
        }
        internal static MvcHtmlString CloseContext()
        {
            if (Instance == null || !Instance.ContextOn) return MvcHtmlString.Create(string.Empty);
            MvcHtmlString res = MvcHtmlString.Create(Instance.ToRender.ToString());
            Instance.ToRender = null;
            Instance.ContextOn = false;
            return res;
        }
        internal static int AddType(Type type, HtmlHelper htmlHelper)
        {
            if (Instance == null) return -1;
            if (!Instance.ContextOn) return -1;
            TransformerInfos res=null;
            bool newItem = false;
            if (!Instance.TransformersDirectory.TryGetValue(type, out res))
            {
                res=new TransformerInfos{Code=Instance.TransformersDirectory.Count, ContextCode=Instance.CurrContext};
                Instance.TransformersDirectory[type] = res;
                newItem = true;
            }
            if (newItem || Instance.CurrContext != res.ContextCode)
            {
                res.ContextCode = Instance.CurrContext;
                Instance.ToRender.AppendFormat(
                    "<input type='hidden' id='{0}' name='{0}' value='{1}' />",
                     ExDefaultBinder.TypeRegisterPrefix+res.Code.ToString(CultureInfo.InvariantCulture),
                     htmlHelper.Encode(Convert.ToString(type.AssemblyQualifiedName)));
            }
            return res.Code;
        }
        
    }
    public static class TransformationContextHelpers
    {
        public static MvcHtmlString OpenTContext(this HtmlHelper htmlHelper)
        {
            TransformationContext.NewContext();
            return MvcHtmlString.Create(string.Empty);
        }
        public static MvcHtmlString CloseTContext(this HtmlHelper htmlHelper)
        {
            return TransformationContext.CloseContext();
        }
    }
}
