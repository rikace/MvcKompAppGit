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
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controls.Bindings;



namespace MVCControlsToolkit.Controls
{
    public static class PagerHelper
    {
        public static MvcHtmlString ThemedPagerFor<VM>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, int>> page,
            Expression<Func<VM, int>> prevPage = null,
            Expression<Func<VM, int>> totalPages = null,
            string name = "SubmitPager"
            )
        {
            if (page == null) throw (new ArgumentNullException("page"));
            
            string themeName = ThemedControlsStrings.GetTheme();
            htmlHelper.ViewData["ThemeParams"] =
               new SubmitPagerDescription
               {
                  Page = page,
                  PrevPage = prevPage,
                  TotalPages = totalPages,
                  HtmlHelper = htmlHelper
               };
            MvcHtmlString res;
            try
            {
                res = htmlHelper.Partial("Themes/" + themeName + "/" + name, htmlHelper.ViewData);
            }
            finally
            {
                htmlHelper.ViewData["ThemeParams"] = null;
            }
            return res;
        }
        public static Pager<VM> PagerFor<VM>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, int>> page,
            Expression<Func<VM, int>> prevPage=null,
            Expression<Func<VM, int>> totalPages=null)
        {
            if (page == null) throw (new ArgumentNullException("page"));
            int? totalPagesValue = null;
            int? prevPageValue = null;
            int pageValue = 0;
            try
            {
                pageValue = page.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch { }
            if (prevPage != null)
            {
                try
                {
                    prevPageValue = prevPage.Compile().Invoke(htmlHelper.ViewData.Model);
                }
                catch { }
            }
            if (totalPages != null)
            {
                try
                {
                    totalPagesValue = totalPages.Compile().Invoke(htmlHelper.ViewData.Model);
                }
                catch { }
            }
            string partialName = ExpressionHelper.GetExpressionText(page);
            string name =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(partialName
                      );
            string prevName = null;
            if (prevPageValue != null && prevPageValue.HasValue)
                prevName=
                    htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      ExpressionHelper.GetExpressionText(prevPage));
            return new Pager<VM>(htmlHelper, name, prevName, pageValue, prevPageValue, totalPagesValue, partialName);
        }
        public static MvcHtmlString ThemedPager<VM>(
           this HtmlHelper<VM> htmlHelper, 
           int currPage,
           string parameterName,
           string actionName,
           int? totPages = null,
           string targetIdName = "",
           string controllerName = null,
           object otherParameters = null,
           string routeName = null,
           string protocol = null,
           string hostname = null,
           string fragment = null,
           string name = "GetPager")
        {

            string themeName = ThemedControlsStrings.GetTheme();
            htmlHelper.ViewData["ThemeParams"] =
               new LinkPagerDescription
               {
                   CurrPage = currPage,
                   ParameterName = parameterName,
                   ActionName = actionName,
                   HtmlHelper = htmlHelper,
                   TotPages = totPages,
                   TargetIdName = targetIdName,
                   ControllerName = controllerName,
                   OtherParameters = otherParameters,
                   RouteName = routeName,
                   Protocol = protocol,
                   Hostname = hostname,
                   Fragment = fragment
               };
            MvcHtmlString res;
            try
            {
                res = htmlHelper.Partial("Themes/" + themeName + "/" + name, htmlHelper.ViewData);
            }
            finally
            {
                htmlHelper.ViewData["ThemeParams"] = null;
            }
            return res;
        }
        public static MvcHtmlString ThemedPager<VM>(
           this HtmlHelper<VM> htmlHelper,
           int currPage,
           string parameterName,
           string actionName,
           System.Web.Routing.RouteValueDictionary otherParameters,
           int? totPages = null,
           string targetIdName = "",
           string controllerName = null,
           string routeName = null,
           string protocol = null,
           string hostname = null,
           string fragment = null,
           string name = "GetPager")
        {

            string themeName = ThemedControlsStrings.GetTheme();
            htmlHelper.ViewData["ThemeParams"] =
               new LinkPagerDescription
               {
                   CurrPage = currPage,
                   ParameterName = parameterName,
                   ActionName = actionName,
                   HtmlHelper = htmlHelper,
                   TotPages = totPages,
                   TargetIdName = targetIdName,
                   ControllerName = controllerName,
                   OtherParameters = otherParameters,
                   RouteName = routeName,
                   Protocol = protocol,
                   Hostname = hostname,
                   Fragment = fragment
               };
            MvcHtmlString res;
            try
            {
                res = htmlHelper.Partial("Themes/" + themeName + "/" + name, htmlHelper.ViewData);
            }
            finally
            {
                htmlHelper.ViewData["ThemeParams"] = null;
            }
            return res;
        }
        public static Pager<VM> Pager<VM>(
           this HtmlHelper<VM> htmlHelper, int currPage,
           string parameterName, 
           string actionName,
           int? totPages = null,
           string targetIdName = "",
           string controllerName = null,
           object otherParameters = null,
           string routeName = null,
           string protocol = null,
           string hostname = null,
           string fragment = null)
        {
            if (parameterName == null) throw (new ArgumentNullException("parameterName"));
            if (actionName == null) throw (new ArgumentNullException("actionName"));
            return new Pager<VM>(htmlHelper, currPage, totPages, parameterName, actionName, targetIdName,
                controllerName, otherParameters, routeName, protocol, hostname, fragment); 
        }
        
        public static Pager<VM> Pager<VM>(
           this HtmlHelper<VM> htmlHelper, int currPage,
           string parameterName,
           string actionName,
           System.Web.Routing.RouteValueDictionary otherParameters,
           int? totPages = null,
           string targetIdName = "",
           string controllerName = null,       
           string routeName = null,
           string protocol = null,
           string hostname = null,
           string fragment = null)
        {
            if (parameterName == null) throw (new ArgumentNullException("parameterName"));
            if (actionName == null) throw (new ArgumentNullException("actionName"));
            return new Pager<VM>(htmlHelper, currPage, totPages, parameterName, actionName, targetIdName,
                controllerName, otherParameters, routeName, protocol, hostname, fragment);
        }

        public static ClientPager<VM> ClientPager<VM> (
           this HtmlHelper<VM> htmlHelper,
           string pagerName,
           int initialPage=1,
           string totalPagesName=null,
           int ? totalPages=null,
           string pagePrefix = null,
           string pagePostfix = null,
           bool causeSubmit=false)
        {
            if (string.IsNullOrWhiteSpace(pagerName)) throw (new ArgumentNullException("pagerName"));
            return new ClientPager<VM>(htmlHelper, pagerName, initialPage, totalPagesName, totalPages, pagePrefix, pagePostfix, causeSubmit);

        }
        public static MvcHtmlString ThemedClientPager<VM>(
           this HtmlHelper<VM> htmlHelper,
           string pagerName,
           int initialPage = 1,
           string totalPagesName = null,
           int? totalPages = null,
           string pagePrefix = null, 
           string pagePostfix = null,
            bool causeSubmit=false,
           string name = "ClientPager")
        {
            if (string.IsNullOrWhiteSpace(pagerName)) throw (new ArgumentNullException("pagerName"));
            string themeName = ThemedControlsStrings.GetTheme();
            htmlHelper.ViewData["ThemeParams"] =
               new ClientPagerDescription
               {
                   PagerName = pagerName,
                   InitialPage = initialPage,
                   TotalPagesName= totalPagesName,
                   TotalPages = totalPages,
                   HtmlHelper = htmlHelper,
                   PagePrefix = pagePrefix,
                   PagePostfix = pagePostfix,
                   CauseSubmit = causeSubmit
               };
            MvcHtmlString res;
            try
            {
                res = htmlHelper.Partial("Themes/" + themeName + "/" + name, htmlHelper.ViewData);
            }
            finally
            {
                htmlHelper.ViewData["ThemeParams"] = null;
            }
            return res;
            

        }
        public static ClientPager<VM>  ClientPagerFor<VM>(
           this HtmlHelper<VM> htmlHelper,
           Expression<Func<VM, int>> page,
           Expression<Func<VM, int>> totPages=null,
           string pagePrefix=null, string pagePostfix=null,
           bool causeSubmit=false)
        {
            if (page == null) throw (new ArgumentNullException("page"));
            string pagerName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                ExpressionHelper.GetExpressionText(page));
            
           int initialPage=1;
           string totalPagesName=null;
           int? totalPages=null;

           try
           {
               initialPage = page.Compile().Invoke(htmlHelper.ViewData.Model);
           }
           catch
           {
           }
           
           if (totPages != null)
           {
               IBindingsBuilder<VM> bindings = htmlHelper.ClientBindings();
               if (bindings == null)
               {
                   totalPagesName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                    ExpressionHelper.GetExpressionText(totPages));
               }
               else
               {
                   totalPagesName = bindings.GetFullBindingName(totPages);
               }
               try
               {
                   totalPages = totPages.Compile().Invoke(htmlHelper.ViewData.Model);
               }
               catch
               {
               }
           }
           return new ClientPager<VM>(htmlHelper, pagerName, initialPage, totalPagesName, totalPages, pagePrefix, pagePostfix, causeSubmit);
        }
        public static MvcHtmlString ThemedClientPagerFor<VM>(
           this HtmlHelper<VM> htmlHelper,
           Expression<Func<VM, int>> page,
           Expression<Func<VM, int>> totPages = null,
           string pagePrefix = null, 
            string pagePostfix = null,
            bool causeSubmit=false,
            string name = "ClientPager")
        {
            if (page == null) throw (new ArgumentNullException("page"));
            string pagerName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                ExpressionHelper.GetExpressionText(page));

            int initialPage = 1;
            string totalPagesName = null;
            int? totalPages = null;

            try
            {
                initialPage = page.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }

            if (totPages != null)
            {
                IBindingsBuilder<VM> bindings = htmlHelper.ClientBindings();
                if (bindings == null)
                {
                    totalPagesName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                     ExpressionHelper.GetExpressionText(totPages));
                }
                else
                {
                    totalPagesName = bindings.GetFullBindingName(totPages);
                }

                try
                {
                    totalPages = totPages.Compile().Invoke(htmlHelper.ViewData.Model);
                }
                catch
                {
                }
            }
            return ThemedClientPager<VM>(htmlHelper,   pagerName, initialPage, totalPagesName, totalPages, pagePrefix, pagePostfix, causeSubmit, name);
        }
    }
    
}
