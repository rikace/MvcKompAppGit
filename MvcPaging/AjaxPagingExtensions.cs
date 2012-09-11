using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MvcAjaxPaging;
using System.Web.Mvc.Ajax;

namespace MvcAjaxPaging
{
	public static class AjaxPagingExtensions
	{
		#region HtmlHelper extensions

        public static MvcHtmlString Pager(this AjaxHelper ajaxHelper, AjaxOptions ajaxOptions, int pageSize, int currentPage, int totalItemCount)
        {
            return Pager(ajaxHelper, ajaxOptions, pageSize, currentPage, totalItemCount, null, null);
        }

        public static MvcHtmlString Pager(this AjaxHelper ajaxHelper, AjaxOptions ajaxOptions, int pageSize, int currentPage, int totalItemCount, string actionName)
        {
            return Pager(ajaxHelper, ajaxOptions, pageSize, currentPage, totalItemCount, actionName, null);
        }

        public static MvcHtmlString Pager(this AjaxHelper ajaxHelper, AjaxOptions ajaxOptions, int pageSize, int currentPage, int totalItemCount, object values)
        {
            return Pager(ajaxHelper, ajaxOptions, pageSize, currentPage, totalItemCount, null, new RouteValueDictionary(values));
        }

        public static MvcHtmlString Pager(this AjaxHelper ajaxHelper, AjaxOptions ajaxOptions, int pageSize, int currentPage, int totalItemCount, string actionName, object values)
        {
            return Pager(ajaxHelper, ajaxOptions, pageSize, currentPage, totalItemCount, actionName, new RouteValueDictionary(values));
        }

        public static MvcHtmlString Pager(this AjaxHelper ajaxHelper, AjaxOptions ajaxOptions, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary)
        {
            return Pager(ajaxHelper, ajaxOptions, pageSize, currentPage, totalItemCount, null, valuesDictionary);
        }

        public static MvcHtmlString Pager(this AjaxHelper ajaxHelper, AjaxOptions ajaxOptions, int pageSize, int currentPage, int totalItemCount, string actionName, RouteValueDictionary valuesDictionary)
        {
            if (valuesDictionary == null)
            {
                valuesDictionary = new RouteValueDictionary();
            }
            if (actionName != null)
            {
                if (valuesDictionary.ContainsKey("action"))
                {
                    throw new ArgumentException("The valuesDictionary already contains an action.", "actionName");
                }
                valuesDictionary.Add("action", actionName);
            }
            var pager = new AjaxPager(ajaxHelper, ajaxHelper.ViewContext, pageSize, currentPage, totalItemCount, ajaxOptions, valuesDictionary);
            return pager.RenderHtml();
        }

		#endregion
	}
}