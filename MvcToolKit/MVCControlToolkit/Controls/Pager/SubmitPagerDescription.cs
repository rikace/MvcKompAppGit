using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MVCControlsToolkit.Controls
{
    public class SubmitPagerDescription
    {
        public dynamic Page { get; set; }
        public dynamic PrevPage { get; set; }
        public dynamic TotalPages { get; set; }
        public dynamic HtmlHelper { get; set; }
    }
    public class ClientPagerDescription
    {
           public dynamic HtmlHelper { get; set; }
           public string PagerName { get; set; }
           public int InitialPage { get; set; }
           public string TotalPagesName { get; set; }
           public int ? TotalPages { get; set; }
           public string PagePrefix  { get; set; }
           public string PagePostfix { get; set; }
           public bool CauseSubmit { get; set; }
    }
    public class LinkPagerDescription
    {
           public int CurrPage { get; set; }
           public string ParameterName { get; set; } 
           public string ActionName { get; set; }
           public dynamic HtmlHelper { get; set; }
           public int? TotPages { get; set; }
           public string TargetIdName { get; set; }
           public string ControllerName { get; set; }
           public object OtherParameters { get; set; }
           public string RouteName { get; set; }
           public string Protocol { get; set; }
           public string Hostname { get; set; }
           public string Fragment { get; set; }
    }
}
