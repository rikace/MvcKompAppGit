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
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using MVCControlsToolkit.Core;
using System.Web.Routing;


namespace MVCControlsToolkit.Controls
{
    public static class MenuHelper
    {
        private static object getTemplate<T>(T item, object itemTemplate, Func<T, int> templateChoice)
        {
            object[] templates = itemTemplate as object[];
            if (templates == null) return itemTemplate;
            return templates[templateChoice == null ? 0 : templateChoice(item)];
           
        }
        private static bool renderMenuItem<T>(HtmlHelper htmlHelper, StringBuilder sb, T item, Func<T, IEnumerable<T>> collectionFunction, object itemTemplate, string prefix, string addPrefix, Func<HtmlHelper, T, bool> selected,
            Func<T, int> templateChoice = null, string selectedClass = "item-selected", string selectedPathClass = "selected-path", string hasChildrenClass = "has-children", Func<T, string> allItemsClassSelector = null, string innerUlClass = null, Func<T, string> liAttributesString = null)
        where T:class, new()
        {
            
            IEnumerable collection = null;
            if (collectionFunction != null)
            {
                try
                {
                    collection = collectionFunction(item);
                }
                catch { }
            }
            else if(item != null)
            {
                collection = (item as SimpleMenuItem).Children;
            }
            bool sel = false;
            if (selected != null)
            {
                sel = selected(htmlHelper, item);
            }
            bool onSelectedPath = false;
            StringBuilder newSb=null;
            string allItemsClass = null;
            if (allItemsClassSelector != null) allItemsClass = allItemsClassSelector(item);
            if (allItemsClass == null) allItemsClass = "menu-node";
            if (collection != null)
            {
                newSb=new StringBuilder();
                var newPrefix = BasicHtmlHelper.AddField(prefix, addPrefix);
                int i = 0;
                if (innerUlClass != null) 
                {
                    newSb.Append("<ul class='"); newSb.Append(innerUlClass); newSb.Append("'>");
                }
                else newSb.Append("<ul>");
                foreach (T innerItem in collection)
                {
                    if (renderMenuItem<T>(htmlHelper, newSb, innerItem, collectionFunction, itemTemplate, string.Format("{0}[{1}]", newPrefix, i), addPrefix, selected,
                        templateChoice, selectedClass, selectedPathClass, hasChildrenClass, allItemsClassSelector, innerUlClass, liAttributesString)) onSelectedPath = true;
                    i++;
                }
                newSb.Append("</ul>");
                
                if (sel)
                {
                    sb.Append("<li class='");
                    sb.Append(allItemsClass);
                    sb.Append(" ");
                    sb.Append(hasChildrenClass);
                    sb.Append(" ");
                    sb.Append(selectedClass);
                    sb.Append("'");
                }
                else if (onSelectedPath)
                {
                    sb.Append("<li class='");
                    sb.Append(allItemsClass);
                    sb.Append(" ");
                    sb.Append(hasChildrenClass);
                    sb.Append(" ");
                    sb.Append(selectedPathClass);
                    sb.Append("'");
                }
                else
                {
                    sb.Append("<li class='");
                    sb.Append(allItemsClass);
                    sb.Append(" ");
                    sb.Append(hasChildrenClass);
                    sb.Append("'");
                }
                if (liAttributesString != null)
                {
                    var toAdd = liAttributesString(item);
                    if (toAdd != null)
                    {
                        sb.Append(" ");
                        sb.Append(toAdd);
                    }
                }
                sb.Append(">");
                sb.Append(new TemplateInvoker<T>(getTemplate(item, itemTemplate, templateChoice)).InvokeNM(htmlHelper, item, prefix));
                sb.Append(newSb);
            }
            else
            {
                if (sel)
                {
                    sb.Append("<li class='");
                    sb.Append(allItemsClass);
                    sb.Append(" ");
                    sb.Append(selectedClass);
                    sb.Append("'");
                }
                else
                {
                    sb.Append("<li class='");
                    sb.Append(allItemsClass);
                    sb.Append("'");
                }
                if (liAttributesString != null)
                {
                    var toAdd = liAttributesString(item);
                    if (toAdd != null)
                    {
                        sb.Append(" ");
                        sb.Append(toAdd);
                    }
                }
                sb.Append(">");
                sb.Append(new TemplateInvoker<T>(getTemplate(item, itemTemplate, templateChoice)).InvokeNM(htmlHelper, item, prefix));
            }
            
            
            
            sb.Append("</li>");
            return sel || onSelectedPath;
        }
        public static MvcHtmlString Menu<T>(this HtmlHelper htmlHelper, string name, IEnumerable<T> collection, IDictionary<string, object> htmlAttributes = null, Expression<Func<T, IEnumerable<T>>> itemCollection = null, object itemTemplate = null, Func<HtmlHelper, T, bool> selected=null,
            Func<T, int> templateChoice = null, string selectedClass = "item-selected", string selectedPathClass = "selected-path", string hasChildrenClass = "has-children", Func<T, string> allItemsClass = null, string innerUlClass = null, Func<T, string> liAttributesString = null)
        where T: class, new()
        {
            if (name == null) throw (new ArgumentNullException("name"));
            if (collection == null)
            {
                    throw (new ArgumentNullException("collection"));
            }
            if (itemCollection == null && typeof(T) != typeof(SimpleMenuItem))
            {
                    throw (new ArgumentNullException("itemCollection"));
            }
            if (itemTemplate == null)
            {
                if (typeof(T) == typeof(SimpleMenuItem))
                {
                    Func<HtmlHelper<SimpleMenuItem>, string> template = x =>
                        {
                            if (x.ViewData.Model.Link == null) return string.Format("<a href='#'>{0}</a>", x.ViewData.Model.Text);
                            else
                            {
                                if (x.ViewData.Model.Target == null)
                                    return string.Format("<a href='{0}'>{1}</a>", x.ViewData.Model.Link, x.ViewData.Model.Text);
                                else
                                    return string.Format("<a href='{0}' target='{2}'>{1}</a>", x.ViewData.Model.Link, x.ViewData.Model.Text, x.ViewData.Model.Target);
                            }

                        };
                    itemTemplate = template;
                }
                else
                {
                    throw (new ArgumentNullException("itemTemplate"));
                }
            }
            StringBuilder sb = new StringBuilder();
            string partialPrefix = name;
            string prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
            string addPrefix = null;
            Func<T, IEnumerable<T>> accessor = null;
            if (itemCollection == null)
                addPrefix = "Children";
            else
            {
                addPrefix = ExpressionHelper.GetExpressionText(itemCollection);
                accessor = itemCollection.Compile();
            }
            sb.AppendFormat("<ul {0}>", BasicHtmlHelper.GetAttributesString(htmlAttributes));
            if (collection != null)
            {
                
                int i=0;
                foreach (T item in collection)
                {
                    renderMenuItem<T>(htmlHelper, sb, item, accessor, itemTemplate, string.Format("{0}[{1}]", prefix, i), addPrefix, selected,
                        templateChoice, selectedClass, selectedPathClass, hasChildrenClass, allItemsClass, innerUlClass, liAttributesString);
                    i++;
                }
            }
            sb.Append("</ul>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString Menu<T>(this HtmlHelper htmlHelper, string name, IEnumerable<T> collection, object htmlAttributes, Expression<Func<T, IEnumerable<T>>> itemCollection = null, object itemTemplate = null,  Func<HtmlHelper, T, bool> selected=null,
            Func<T, int> templateChoice = null, string selectedClass = "item-selected", string selectedPathClass = "selected-path", string hasChildrenClass = "has-children", Func<T, string> allItemsClass = null, string innerUlClass = null, Func<T, string> liAttributesString = null)
        where T : class, new()
        {
            return Menu(htmlHelper, name, collection, new RouteValueDictionary(htmlAttributes), itemCollection, itemTemplate, selected,
                templateChoice, selectedClass, selectedPathClass, hasChildrenClass, allItemsClass, innerUlClass, liAttributesString);
        }
        public static MvcHtmlString MenuFor<M, T>(this HtmlHelper<M> htmlHelper, Expression<Func<M, IEnumerable<T>>> rootCollection, IDictionary<string, object> htmlAttributes = null, Expression<Func<T, IEnumerable<T>>> itemCollection = null, object itemTemplate = null, Func<HtmlHelper, T, bool> selected = null,
            Func<T, int> templateChoice = null, string selectedClass = "item-selected", string selectedPathClass = "selected-path", string hasChildrenClass = "has-children", Func<T, string> allItemsClass = null, string innerUlClass = null, Func<T, string> liAttributesString = null)
        where T : class, new()
        {
            if (rootCollection == null) throw (new ArgumentException("rootCollection"));
            string name = ExpressionHelper.GetExpressionText(rootCollection);
            IEnumerable<T> collection = null;
            try
            {
                collection = rootCollection.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch { }
            return Menu(htmlHelper, name, collection, htmlAttributes, itemCollection, itemTemplate, selected,
                templateChoice, selectedClass, selectedPathClass, hasChildrenClass, allItemsClass, innerUlClass, liAttributesString);
        }
        public static MvcHtmlString MenuFor<M, T>(this HtmlHelper<M> htmlHelper, Expression<Func<M, IEnumerable<T>>> rootCollection, object htmlAttributes, Expression<Func<T, IEnumerable<T>>> itemCollection = null, object itemTemplate = null,
            Func<HtmlHelper, T, bool> selected = null, Func<T, int> templateChoice = null, string selectedClass = "item-selected", string selectedPathClass = "selected-path", string hasChildrenClass = "has-children", Func<T, string>  allItemsClass = null, string innerUlClass=null, Func<T, string> liAttributesString=null)
        where T : class, new()
        {
            return MenuFor(htmlHelper, rootCollection, new RouteValueDictionary(htmlAttributes), itemCollection, itemTemplate, selected,
                templateChoice, selectedClass, selectedPathClass, hasChildrenClass, allItemsClass, innerUlClass, liAttributesString);
        }
    }
}
