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
using MVCControlsToolkit.DataAnnotations;
using MVCControlsToolkit.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Globalization;


namespace MVCControlsToolkit.Controls
{
    public class DateTimeInput<VM>:IDisplayModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        private DateRangeAttribute[] constraints;
        public DateTime? Minimum
        {
            get
            {
                return min;
            }
        }
        public DateTime? Maximum
        {
            get
            {
                return max;
            }
        }
        DateTime? min = null;
        DateTime? max = null;
        DateTime? currMin = null;
        DateTime? currMax = null;
        DateTime? curr;
        private Type containerType;
        private bool baseScriptsRendered;
        private HtmlHelper<VM> htmlHelper;
        private string prefix;
        private string totalPrefix;
        private string clientOnChanged;
        private string clientMinScript=string.Empty;
        private string clientMaxScript=string.Empty;
        private string clientRefreshRegistrations = string.Empty;
        private bool yearCombo = false;
        private bool dateHidden = false;
        private bool dateInCalendar;
        private PropertyAccessor pAccesor = null;
        private bool dateRendered = false;
        private bool timeRendered = false;
        private string Role=null;
        private bool functionWritten = false;
        private IDictionary<string, object> attributeExtensions;
        private string clientFormat;
        private static string startScriptFormat=
        @"
            <span {11} style='display:none' id='{0}_Hidden' pname='{12}' data-element-type = 'DateTimeInput' data-client-type='4'></span>
            <script language='javascript' type='text/javascript'>
                var {0}_Curr = {4};
                var {0}_MaxDate = {2};
                var {0}_MinDate = {1};
                var {0}Recursive = false;
                var {0}_Valid = true;
                var {0}_YearCombo = {7};
                var {0}_DateHidden = {8};
                var {0}_DateInCalendar = {10};
                var {0}_ClientDateChanged = 
                    function(date)
                    {{
                        {3}
                    }};
                var {0}_ClientDynamicMin = 
                    function()
                    {{
                        var min=null;
                        var cmin=null;
                        {5}
                        return min;
                    }};
                var {0}_ClientDynamicMax = 
                    function()
                    {{
                        var max=null;
                        var cmax=null;
                        {6}
                        return max;
                    }};
                {9}
                $(document).ready(function()
                {{
                    DateInput_Initialize('{0}');
                }});
            </script>
        ";

        string updateSchema = @"
        if (typeof {0}_UpdateListIndex === 'undefined')
        {{
            var {0}_UpdateListIndex = 0;
            var {0}_UpdateList = new Array();
        }}
        AddToUpdateList('{0}', '{1}'); 
        ";
        string calendarSchema = @"
            <div id='{0}_Calendar' {2}></div>
            <script language='javascript' type='text/javascript'>
             var {0}_InLine=true;   
             var {0}_CalendarOptions = {1};
            </script>
        ";
        string calendarSchemaOffLine = @"
            <input type='text' data-elementispart = 'true' id='{0}_Calendar' {2}/>
            <script language='javascript' type='text/javascript'>
                var {0}_CalendarText=null;
                
             $('#{0}_Calendar').focus(
                function(){{
                    {0}_CalendarText=this.value;
                    
                }}
             );
             var {0}_InLine=false;   
             var {0}_CalendarOptions = {1};
            </script>
        ";
        public DateTimeInput()
        {
        }
        public DateTimeInput(HtmlHelper<VM> htmlHelper, string prefix, Nullable<DateTime> date, bool dateInCalendar = false, IDictionary<string, object> attributeExtensions = null)
        {
            this.htmlHelper=htmlHelper;
            this.prefix=prefix;
            this.dateInCalendar = dateInCalendar;
            this.attributeExtensions = attributeExtensions;
            this.totalPrefix = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(prefix);
            PropertyAccessor prop = new PropertyAccessor(prefix, typeof(VM));
            pAccesor = prop;
            FormatAttribute[] fa = prop[typeof(FormatAttribute)] as FormatAttribute[];
            string fPrefix = string.Empty;
            string postfix = string.Empty;
            clientFormat = null;
            if (fa != null && fa.Length > 0)
            {
                fa[0].GetClientFormat(out fPrefix, out postfix, out clientFormat);
            }
            constraints = prop[typeof(DateRangeAttribute)] as DateRangeAttribute[];
            if (constraints != null)
            {
                foreach (DateRangeAttribute c in constraints)
                {
                    if (c.Maximum.HasValue)
                    {
                        if (max.HasValue)
                        {
                            if (max.Value > c.Maximum) max = c.Maximum;
                        }
                        else max = c.Maximum;
                    }
                    if (c.Minimum.HasValue)
                    {
                        if (min.HasValue)
                        {
                            if (min.Value < c.Minimum) min = c.Minimum;
                        }
                        else min = c.Minimum;
                    }
                }
            }
            if(max !=null && max.HasValue && min != null && min.HasValue && min.Value>max.Value)
                max=min;
            curr=date;
            if (min != null && min.HasValue && (curr == null || !curr.HasValue || curr < min))
                curr = min;
            if (max != null && max.HasValue && (curr == null || !curr.HasValue || curr > max))
                curr = max;
            ComputeClientRangeScript();
            ComputeClientOnChanged();
            ImportFromModel(curr);
            if (prop.Property != null) containerType = prop.Property.DeclaringType;
            
        }

        public object ExportToModel(Type TargetType, params object[] context)
        {
            if (Year == 0 && Month == 0 && Day == 0 && Hours == 0 && Minutes == 0 && Seconds == 0)
                return null;
            if (Year == 0) Year = default(DateTime).Year;
            if (Month == 0) Month = default(DateTime).Month;
            if (Day == 0) Day = default(DateTime).Day;

            try
            {
                return new DateTime(Year, Month, Day, Hours, Minutes, Seconds);
            }
            catch (Exception ex)
            {
                throw (new Exception(" {0} has an incorrect date format", ex));
                
            } 
            
        }

        public void ImportFromModel(object model, params object[] context)
        {
            DateTime? value = model as DateTime?;

            if (value != null && value.HasValue)
            {
                Year = value.Value.Year;
                Month = value.Value.Month;
                Day = value.Value.Day;

                Hours = value.Value.Hour;
                Minutes = value.Value.Minute;
                Seconds = value.Value.Second;
                
            }
        }
        public MvcHtmlString DateCalendar(CalendarOptions calendarOptions=null, bool inLine=true, IDictionary<string, object> containerHtmlAttributes=null)
        {
            if (this.dateRendered) return MvcHtmlString.Create(string.Empty);
            if (!this.dateInCalendar) throw new ArgumentException(ControlsResources.DateTimeInput_Calendar);
            
            dateRendered = true;
            string newPrefix = BasicHtmlHelper.AddField(prefix, "$");

            

            StringBuilder sb = new StringBuilder();
           
            sb.Append(htmlHelper.Hidden(
                    BasicHtmlHelper.AddField(newPrefix, "Year"), Year).ToString());
            sb.Append(htmlHelper.Hidden(
                    BasicHtmlHelper.AddField(newPrefix, "Month"), Month).ToString());
            sb.Append(htmlHelper.Hidden(
                    BasicHtmlHelper.AddField(newPrefix, "Day"), Day).ToString());
            if (calendarOptions == null) calendarOptions= new CalendarOptions();
            string clientId=BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(totalPrefix, "$"));
            if (string.IsNullOrWhiteSpace(calendarOptions.DateFormat))
            {
                calendarOptions.DateFormat = clientFormat;
            }
            clientFormat = calendarOptions.DateFormat;
            switch (clientFormat)
            {
                case "d": calendarOptions.DateFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    break;
                case "f": calendarOptions.DateFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern;
                    break;
                case "F": calendarOptions.DateFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern;
                    break;
                case "D": calendarOptions.DateFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern;
                    break;
                case "M": calendarOptions.DateFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern;
                    break;
                case "Y": calendarOptions.DateFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern;
                    break;
                case "S": calendarOptions.DateFormat = "yyyy-MM-dd";
                    break;
                case "t": calendarOptions.DateFormat = null;
                    break;
                case "T": calendarOptions.DateFormat = null;
                    break;
                default:
                    calendarOptions.DateFormat = null;
                    break;
            }
            if (calendarOptions.DateFormat != null)
                calendarOptions.DateFormat = calendarOptions.DateFormat
                    .Replace("yy", "y")
                    .Replace("dddd", "DD")
                    .Replace("ddd", "D")
                    .Replace("M", "m")
                    .Replace("mmmm", "MM")
                    .Replace("mmm", "M");
            //AddFunctions(ref containerHtmlAttributes, true);
            if (inLine)
            {
                sb.Append(string.Format(calendarSchema,
                    clientId,
                    calendarOptions.Render(clientId, inLine), BasicHtmlHelper.GetAttributesString(containerHtmlAttributes)));
            }
            else
            {
                sb.Append(string.Format(calendarSchemaOffLine,
                    clientId,
                    calendarOptions.Render(clientId, inLine), BasicHtmlHelper.GetAttributesString(containerHtmlAttributes)));
            }
            sb.Append(RenderBasicScripts(null));
            return MvcHtmlString.Create(sb.ToString());

        }
        public MvcHtmlString Date(bool useTextBoxForYear = false, IDictionary<string, object> htmlAttributes = null, IDictionary<string, object> htmlAttributesTextBox = null, CultureInfo culture = null)
        {
            if (this.dateRendered) return MvcHtmlString.Create(string.Empty);
            if (this.dateInCalendar) throw new ArgumentException(ControlsResources.DateTimeInput_Combo);
            dateRendered = true;
            string newPrefix = BasicHtmlHelper.AddField(prefix, "$");
            if (culture == null) culture = CultureInfo.CurrentUICulture;
            if (htmlAttributesTextBox == null) htmlAttributesTextBox = htmlAttributes;
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            if (htmlAttributesTextBox == null) htmlAttributesTextBox = new Dictionary<string, object>();
            htmlAttributesTextBox["data-elementispart"]="true";
            htmlAttributes["data-elementispart"] = "true";
            DateTime? min = currMin;
            DateTime? max = currMax;
            StringBuilder sb = new StringBuilder();

            
            BasicHtmlHelper.SetDefaultStyle(htmlAttributes, "text-align", "right");
            string sYear =string.Empty;
            
            if (useTextBoxForYear || min == null || !min.HasValue || max == null || !max.HasValue)
            {
                htmlAttributesTextBox["maxlength"] = "4";
                htmlAttributesTextBox["size"] = "5";
                //AddFunctions(ref htmlAttributesTextBox, false);
                BasicHtmlHelper.SetDefaultStyle(htmlAttributesTextBox, "text-align", "right");
                BasicHtmlHelper.SetDefaultStyle(htmlAttributesTextBox, "width", "4em");
                sYear = htmlHelper.TextBox(
                    BasicHtmlHelper.AddField(newPrefix, "Year"), Year, htmlAttributesTextBox).ToString();
            }
            else
            {
                
                
                yearCombo = true;
                List<SelectListItem> allYears = new List<SelectListItem>(1);
                for (int year = Year; year <= Year; year++)
                {
                    allYears.Add(new SelectListItem()
                        {
                            Value = year.ToString(),
                            Text = AlignFourDigits(year),
                            Selected = Year == year
                        });
                }
                //AddFunctions(ref htmlAttributes, false);
                sYear = htmlHelper.DropDownList(
                    BasicHtmlHelper.AddField(newPrefix, "Year"),
                    allYears,
                    htmlAttributes).ToString();
            }
            sb.Append(RenderBasicScripts(culture));
            int minMonth =1;
            int maxMonth = 12;
            if (min!=null  && min.HasValue  && min.Value.Year==curr.Value.Year)
            {
                if(min.Value.Month>minMonth) minMonth=min.Value.Month;
                
            }
            if (max != null  && max.HasValue &&  curr.Value.Year == max.Value.Year)
            {
                if (max.Value.Month < maxMonth) maxMonth = max.Value.Month;
            }
            List<SelectListItem> allMonths = new List<SelectListItem>(1);
            for (int month = Month; month <= Month; month++)
            {
                allMonths.Add(new SelectListItem()
                        {
                            Value=month.ToString(),
                            Text = culture.DateTimeFormat.MonthNames[month-1],
                            Selected = Month == month
                        });
            }
            //AddFunctions(ref htmlAttributes, false);
            string sMonth = htmlHelper.DropDownList(
                    BasicHtmlHelper.AddField(newPrefix, "Month"),
                    allMonths,
                    htmlAttributes).ToString();
            int minDay =1;
            int maxDay = DateTime.DaysInMonth(Year, Month);
            
            if (min!=null  && min.HasValue &&  
                min.Value.Year==curr.Value.Year && min.Value.Month==curr.Value.Month)
            {   
                if(min.Value.Day>minDay) minDay=min.Value.Day;
            }
            if (max != null && max.HasValue &&
                curr.Value.Year == max.Value.Year && curr.Value.Month == max.Value.Month)
            {
                if (max.Value.Day < maxDay) maxDay = max.Value.Day;
            }
            List<SelectListItem> allDays = new List<SelectListItem>(1);
            for (int day = Day; day <= Day; day++)
            {
                allDays.Add(new SelectListItem()
                        {
                            Value=day.ToString(),
                            Text = AlignTwoDigits(day),
                            Selected = Day == day
                        });
            }
            //AddFunctions(ref htmlAttributes, true);
            string sDay = htmlHelper.DropDownList(
                    BasicHtmlHelper.AddField(newPrefix, "Day"),
                    allDays,
                    htmlAttributes).ToString();
            char[] OrderInfo = dateOrder(culture);
            
            if (OrderInfo[0]=='y') sb.Append(sYear);
            if (OrderInfo[0]=='M') sb.Append(sMonth);
            if (OrderInfo[0]=='d') sb.Append(sDay);

            if (OrderInfo[1]=='y') sb.Append(sYear);
            if (OrderInfo[1]=='M') sb.Append(sMonth);
            if (OrderInfo[1]=='d') sb.Append(sDay);

            if (OrderInfo[2]=='y') sb.Append(sYear);
            if (OrderInfo[2]=='M') sb.Append(sMonth);
            if (OrderInfo[2]=='d') sb.Append(sDay);

            return MvcHtmlString.Create(sb.ToString());
        }
        protected string AlignTwoDigits(int x)
        {
            return (x < 10) ? " " + x.ToString() : x.ToString();
        }
        protected string AlignFourDigits(int x)
        {
            if (x < 10) return "   " + x.ToString() ;
            else if (x < 100) return "  " + x.ToString();
            else if (x < 1000) return " " + x.ToString();
            else return x.ToString();
        }

        public MvcHtmlString Time(bool dateHidden = false, IDictionary<string, object> htmlAttributes = null, CultureInfo culture = null)
        {

            if (timeRendered) return MvcHtmlString.Create(string.Empty);
            timeRendered = true;
            this.dateHidden = dateHidden;
            if (culture == null) culture = CultureInfo.CurrentUICulture;
            string newPrefix = BasicHtmlHelper.AddField(prefix, "$");
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            htmlAttributes["data-elementispart"] = "true";
            DateTime? min = currMin;
            DateTime? max = currMax;

            StringBuilder sb = new StringBuilder();

            sb.Append(RenderBasicScripts(culture));
            htmlAttributes["maxlength"] = "2";
            BasicHtmlHelper.SetDefaultStyle(htmlAttributes, "text-align", "right");
            if(dateHidden)
            {
                sb.Append(htmlHelper.Hidden(
                    BasicHtmlHelper.AddField(newPrefix, "Year"), Year).ToString());
                sb.Append(htmlHelper.Hidden(
                    BasicHtmlHelper.AddField(newPrefix, "Month"), Month).ToString());
                sb.Append(htmlHelper.Hidden(
                    BasicHtmlHelper.AddField(newPrefix, "Day"), Day).ToString());
            }
            

            string separator = culture.DateTimeFormat.TimeSeparator;
           
            bool applyRestrictionsMin = min != null && min.HasValue &&
                min.Value.Year == curr.Value.Year && min.Value.Month == curr.Value.Month &&
                min.Value.Day == curr.Value.Day;

            bool applyRestrictionsMax = max != null && max.HasValue &&
                curr.Value.Year == max.Value.Year && curr.Value.Month == max.Value.Month &&
                curr.Value.Day == max.Value.Day;

            int minHour = 0;
            int maxHour = 23;

            if (applyRestrictionsMin)
            {
                if (min.Value.Hour > minHour) minHour = min.Value.Hour;

            }
            else
                applyRestrictionsMin = false;
            if (applyRestrictionsMax)
            {
                if (max.Value.Hour < maxHour) maxHour = max.Value.Hour;
            }
            else
                applyRestrictionsMax = false;

            List<SelectListItem> allHours = new List<SelectListItem>(1);
            for (int hour = Hours; hour <= Hours; hour++)
            {
                allHours.Add(new SelectListItem()
                {
                    Value = hour.ToString(),
                    Text = hour.ToString("00"),
                    Selected = Hours == hour
                });
            }
            //AddFunctions(ref htmlAttributes, false);
            sb.Append(htmlHelper.DropDownList(
                    BasicHtmlHelper.AddField(newPrefix, "Hours"),
                    allHours,
                    htmlAttributes).ToString());
            sb.Append(string.Format("<span class='{0}'>{1}</span>", htmlAttributes["style"].ToString(), separator));
            int minMinute = 0;
            int maxMinute = 59;

            if (applyRestrictionsMin)
            {
                if (min.Value.Minute > minMinute) minMinute = min.Value.Minute;
            }
            else
                applyRestrictionsMin = false;

            if (applyRestrictionsMax)
            {
                if (max.Value.Minute < maxMinute) maxMinute = max.Value.Minute;
            }
            else
                applyRestrictionsMax = false;

            List<SelectListItem> allMinutes = new List<SelectListItem>(1);
            for (int minute = Minutes; minute <= Minutes; minute++)
            {
                allMinutes.Add(new SelectListItem()
                {
                    Value = minute.ToString(),
                    Text = minute.ToString("00"),
                    Selected = Minutes == minute
                });
            }
            //AddFunctions(ref htmlAttributes, false);
            sb.Append(htmlHelper.DropDownList(
                    BasicHtmlHelper.AddField(newPrefix, "Minutes"),
                    allMinutes,
                    htmlAttributes).ToString());

            sb.Append(string.Format("<span class='{0}'>{1}</span>", htmlAttributes["style"].ToString(), separator));
            int minSecond = 0;
            int maxSecond = 59;

            if (applyRestrictionsMin)
            {
                if (min.Value.Second > minSecond) minSecond = min.Value.Second;
            }
            else
                applyRestrictionsMin=false;

            if (applyRestrictionsMax)
            {
                if (max.Value.Second < maxSecond) maxSecond = max.Value.Second;
            }
            else
                applyRestrictionsMax = false;

            List<SelectListItem> allSeconds = new List<SelectListItem>(1);
            for (int second = Seconds; second <= Seconds; second++)
            {
                allSeconds.Add(new SelectListItem()
                {
                    Value = second.ToString(),
                    Text = second.ToString("00"),
                    Selected = Seconds == second
                });
            }
            //AddFunctions(ref htmlAttributes, true);
            sb.Append(htmlHelper.DropDownList(
                    BasicHtmlHelper.AddField(newPrefix, "Seconds"),
                    allSeconds,
                    htmlAttributes).ToString());

            return MvcHtmlString.Create(sb.ToString());
            
        }
        protected void AddFunctions(ref IDictionary<string, object> htmlAttributes, bool chosen)
        {
            if(! chosen && htmlAttributes != null && htmlAttributes.ContainsKey("role-name"))
            {
                Role=htmlAttributes["role-name"] as string;
                htmlAttributes.Remove("role-name");
            }
            if (! chosen) return;
            if (functionWritten) return;
            if (htmlAttributes == null) htmlAttributes= new Dictionary<string, object>();
            if (Role != null)
            {
                htmlAttributes["role-name"]=Role;
                Role=null;
            }
            
            string id=BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(this.totalPrefix, "$"));
            htmlAttributes["mvc-controls-toolkit-set"] = 
                string.Format("function(x) {{MvcControlsToolkit_DateTime_Set('{0}', x);}}",
                id);
            htmlAttributes["mvc-controls-toolkit-get-true"] =
                string.Format("MvcControlsToolkit_DateTime_GetTrue('{0}')",
                id);
            functionWritten=true;
        }
        protected string RenderBasicScripts(CultureInfo format)
        {
            if(baseScriptsRendered) return string.Empty;
            baseScriptsRendered=true;
            
            StringBuilder sb=new StringBuilder();
            sb.Append(BasicHtmlHelper.RenderDisplayInfo(htmlHelper, this.GetType(), prefix));
            bool isClientBlock = htmlHelper.ViewData["ClientBindings"] != null
                    || htmlHelper.ViewContext.HttpContext.Items.Contains("ClientTemplateOn");
            sb.Append(string.Format(startScriptFormat,
                BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(this.totalPrefix, "$")),
                BasicHtmlHelper.JavaScriptDate(min, isClientBlock),
                BasicHtmlHelper.JavaScriptDate(max, isClientBlock),
                clientOnChanged != null ? clientOnChanged+" return" : " return;",
                BasicHtmlHelper.JavaScriptDate(curr),
                clientMinScript,
                clientMaxScript,
                yearCombo ? "true" : "false",
                dateHidden ? "true" : "false",
                clientRefreshRegistrations,
                this.dateInCalendar ? "true" : "false",
                BasicHtmlHelper.GetAttributesString(attributeExtensions),
                totalPrefix
                ));
            sb.Append(BasicHtmlHelper.TrueValue(htmlHelper, this.totalPrefix, curr));
            FormatAttribute[] formatAttributes = pAccesor[typeof(FormatAttribute)] as FormatAttribute[];
            if (formatAttributes != null && formatAttributes.Length > 0)
            {
                sb.Append(BasicHtmlHelper.FormattedValueString(htmlHelper, this.totalPrefix, formatAttributes[0].HtmlEncode ? htmlHelper.Encode(formatAttributes[0].GetDisplay(curr)) : formatAttributes[0].GetDisplay(curr)));
            }
            else
            {
                DisplayFormatAttribute[] dFormatAttributes = pAccesor[typeof(DisplayFormatAttribute)] as DisplayFormatAttribute[];
                if (dFormatAttributes != null && dFormatAttributes.Length > 0)
                {
                    sb.Append(BasicHtmlHelper.FormattedValueString(htmlHelper, this.totalPrefix, dFormatAttributes[0].HtmlEncode ? htmlHelper.Encode(
                        new FormatAttribute(dFormatAttributes[0]).GetDisplay(curr)) : new FormatAttribute(dFormatAttributes[0]).GetDisplay(curr)));
                }

            }
            if (format != null) WriteMonthes(sb, format);
            return sb.ToString();
        }

        protected void ComputeClientRangeScript ()
        {
            if (constraints==null || constraints.Length == 0) return;
            StringBuilder sbMin = new StringBuilder();
            StringBuilder sbMax = new StringBuilder();
            StringBuilder sUp = new StringBuilder();
            string prefix = this.totalPrefix;
            string fatherPrefix = BasicHtmlHelper.PreviousPrefix(prefix);
            currMin = min;
            currMax = max;
            string myClientId=BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$"));
            foreach(DateRangeAttribute c in constraints)
            {
                c.RetriveDynamicDelays(htmlHelper.ViewData.Model, fatherPrefix);
                if (!string.IsNullOrWhiteSpace(c.DynamicMaximum) && c.RangeAction==RangeAction.Verify)
                {
                    bool isMilestone = false;
                    string path = (fatherPrefix.Length != 0 ? fatherPrefix + "." + c.DynamicMaximum : c.DynamicMaximum);
                    string clientId = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(path, "$"));
                    PropertyAccessor dynamicMaximumProp = new PropertyAccessor(htmlHelper.ViewData.Model,
                        path, false);
                    TimeSpan delay = new TimeSpan(0L);
                    if (c.MaximumDelay != null && c.MaximumDelay.HasValue) delay = c.MaximumDelay.Value;
                    if (dynamicMaximumProp != null && dynamicMaximumProp.Value != null)
                    {

                        MileStoneAttribute[] ma = dynamicMaximumProp[typeof(MileStoneAttribute)] as MileStoneAttribute[];
                        if (ma != null && ma.Length > 0)
                        {
                            isMilestone = true;
                            if (dynamicMaximumProp.Property.PropertyType == typeof(DateTime))
                            {
                                if (max == null || !max.HasValue || (((DateTime)dynamicMaximumProp.Value).Add(delay) < max))
                                {
                                    max = ((DateTime)dynamicMaximumProp.Value).Add(delay);
                                }
                            }
                            else if (dynamicMaximumProp.Property.PropertyType == typeof(Nullable<DateTime>))
                            {
                                Nullable<DateTime> pmax = dynamicMaximumProp.Value as Nullable<DateTime>;
                                if (pmax.HasValue) pmax = pmax.Value.Add(delay);
                                if (pmax.HasValue && (max == null || !max.HasValue || pmax < currMax))
                                    currMax = pmax;

                            }
                        }
                        if (dynamicMaximumProp.Property.PropertyType == typeof(DateTime))
                        {
                            if (currMax == null || !currMax.HasValue || (((DateTime)dynamicMaximumProp.Value).Add(delay) < currMax))
                            {
                                currMax = ((DateTime)dynamicMaximumProp.Value).Add(delay);
                            }
                        }
                        else if (dynamicMaximumProp.Property.PropertyType == typeof(Nullable<DateTime>))
                        {
                            Nullable<DateTime> pmax = dynamicMaximumProp.Value as Nullable<DateTime>;
                            if(pmax.HasValue)pmax = pmax.Value.Add(delay);
                            if (pmax.HasValue && (currMax == null || !currMax.HasValue || pmax < currMax))
                                currMax = pmax;

                        }
                        else
                            throw (new InvalidDynamicRangeException(AnnotationsRsources.InvalidUpperDynamicRange));

                    }
                    if (!isMilestone)
                    {
                        string varId =
                            BasicHtmlHelper.IdFromName(
                            (fatherPrefix.Length != 0 ? fatherPrefix + "." + c.DynamicMaximum : c.DynamicMaximum) + ".$.Curr");

                        sbMax.Append(string.Format("if ({0} != null){{ cmax = new Date({0}.getTime()+{1});  if (max == null || cmax < max) max = cmax;}}",
                            varId, delay.Ticks / 10000));
                        sUp.Append(string.Format(updateSchema, clientId, myClientId));
                    }
                }
                if (!string.IsNullOrWhiteSpace(c.DynamicMinimum) && c.RangeAction == RangeAction.Verify)
                {

                    bool isMilestone = false;
                    string path = (fatherPrefix.Length != 0 ? fatherPrefix + "." + c.DynamicMinimum : c.DynamicMinimum);
                    string clientId = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(path, "$"));
                    PropertyAccessor dynamicMinimumProp = new PropertyAccessor(htmlHelper.ViewData.Model,
                      path, false);
                    TimeSpan delay = new TimeSpan(0L);
                    if (c.MinimumDelay != null && c.MinimumDelay.HasValue) delay = c.MinimumDelay.Value;
                    if (dynamicMinimumProp != null && dynamicMinimumProp.Value != null)
                    {
                        MileStoneAttribute[] ma = dynamicMinimumProp[typeof(MileStoneAttribute)] as MileStoneAttribute[];
                        if (ma != null && ma.Length > 0)
                        {
                            isMilestone = true;
                            if (dynamicMinimumProp.Property.PropertyType == typeof(DateTime))
                            {
                                if (min == null || !min.HasValue || (((DateTime)dynamicMinimumProp.Value).Add(delay) > min))
                                {
                                    min = ((DateTime)dynamicMinimumProp.Value).Add(delay);
                                }
                            }
                            else if (dynamicMinimumProp.Property.PropertyType == typeof(Nullable<DateTime>))
                            {
                                Nullable<DateTime> pmin = dynamicMinimumProp.Value as Nullable<DateTime>;
                                if (pmin.HasValue) pmin = pmin.Value.Add(delay);
                                if (pmin.HasValue && (min == null || !min.HasValue || pmin > min))
                                    min = pmin;

                            }
                        }
                        if (dynamicMinimumProp.Property.PropertyType == typeof(DateTime))
                        {
                            if (currMin == null || !currMin.HasValue || (((DateTime)dynamicMinimumProp.Value).Add(delay) > currMin))
                            {
                                currMin = ((DateTime)dynamicMinimumProp.Value).Add(delay);
                            }
                        }
                        else if (dynamicMinimumProp.Property.PropertyType == typeof(Nullable<DateTime>))
                        {
                            Nullable<DateTime> pmin = dynamicMinimumProp.Value as Nullable<DateTime>;
                            if (pmin.HasValue) pmin = pmin.Value.Add(delay);
                            if (pmin.HasValue && (currMin == null || !currMin.HasValue || pmin > currMin))
                                currMin = pmin;

                        }
                        else
                            throw (new InvalidDynamicRangeException(AnnotationsRsources.InvalidUpperDynamicRange));

                    }
                    if (!isMilestone)
                    {
                        string varId =
                            BasicHtmlHelper.IdFromName(
                            (fatherPrefix.Length != 0 ? fatherPrefix + "." + c.DynamicMinimum : c.DynamicMinimum) + ".$.Curr");

                        sbMin.Append(string.Format("if ({0} != null){{ cmin = new Date({0}.getTime()+{1});  if (min == null || cmin > min) min = cmin;}}",
                            varId, delay.Ticks / 10000));

                        sUp.Append(string.Format(updateSchema, clientId, myClientId));
                    }
                }
            }
            if (currMin != null && currMin.HasValue && curr != null && curr.HasValue && curr < currMin) curr = currMin;
            if (currMax != null && currMax.HasValue && curr != null && curr.HasValue && curr > currMax) curr = currMax;
            clientMinScript = sbMin.ToString();
            clientMaxScript = sbMax.ToString();
            clientRefreshRegistrations = sUp.ToString();
        }

        protected void ComputeClientOnChanged()
        {
            string prefix = this.totalPrefix;
            if (constraints == null || constraints.Length == 0) return;
            StringBuilder sb = new StringBuilder();
            string fatherPrefix = BasicHtmlHelper.PreviousPrefix(prefix);
            
            foreach(DateRangeAttribute c in constraints)
            {
                
                c.RetriveDynamicDelays(htmlHelper.ViewData.Model, fatherPrefix);
                if (!string.IsNullOrWhiteSpace(c.DynamicMaximum) && c.RangeAction==RangeAction.Propagate)
                {
                    
                    string controlId=
                        BasicHtmlHelper.IdFromName(
                        (fatherPrefix.Length!= 0 ? fatherPrefix+"."+c.DynamicMaximum : c.DynamicMaximum)+".$");
                    long delay=0;
                    if (c.MaximumDelay != null && c.MaximumDelay.HasValue) delay=
                        c.MaximumDelay.Value.Ticks/10000L;
                    sb.Append(string.Format("SetDateInput('{0}', date - {1}, 2); ", controlId, delay));
                }
                if (!string.IsNullOrWhiteSpace(c.DynamicMinimum) && c.RangeAction == RangeAction.Propagate)
                {
                    
                    string controlId =
                        BasicHtmlHelper.IdFromName(
                        (fatherPrefix.Length != 0 ? fatherPrefix + "." + c.DynamicMinimum : c.DynamicMinimum) + ".$");
                    long delay = 0;
                    if (c.MinimumDelay != null && c.MinimumDelay.HasValue) delay =
                        c.MinimumDelay.Value.Ticks / 10000L;
                    sb.Append(string.Format("SetDateInput('{0}', date - {1}, 1); ", controlId, delay));
                }
            }
            clientOnChanged=sb.ToString();
        }

        protected void WriteMonthes(StringBuilder builder1, CultureInfo format)
        {
            if (htmlHelper.ViewContext.HttpContext.Items.Contains("DateTimeInputRendered"))
                return;
            else
                htmlHelper.ViewContext.HttpContext.Items.Add("DateTimeInputRendered", new object());


            string[] Monthes = format.DateTimeFormat.MonthNames;
            htmlHelper.DeclareStringArray(Monthes, "DateTimeMonthes");
            if (htmlHelper.ViewContext.HttpContext.Items.Contains("ClientTemplateOn"))
            {
                htmlHelper.GetStringArray("DateTimeMonthes");
            }
            else
            {
                builder1.Append(htmlHelper.ToJavaScript(Monthes, "DateTimeMonthes"));
            }
            /*  builder1.Append(" <script language='javascript' type='text/javascript'> ");
              builder1.Append("var DateTimeMonthes = new Array(\"");
              builder1.Append(Monthes[0]);
              builder1.Append("\"");
              for (int i = 1; i < Monthes.Length; i++)
              {
                  builder1.Append(", \"");
                  builder1.Append(Monthes[i]);
                  builder1.Append("\"");
              }
              builder1.Append(");");
              builder1.Append("</script> ");
             */
        }
        private char[] dateOrder(CultureInfo format)
        {
            string patt = format.DateTimeFormat.LongDatePattern;
            //compute date order
            patt = (patt.Replace("dddd", " ")).Replace("ddd", " ");
            char[] DateOrder = new char[3];
            int CurrIndex = 0;
            bool DayNotFound = true;
            bool YearNotFound = true;
            bool MonthNotFound = true;
            for (int i = 0; i < patt.Length && CurrIndex < 3; i++)
            {
                if (DayNotFound && patt[i] == 'd')
                {
                    DayNotFound = false;
                    DateOrder[CurrIndex] = 'd';
                    CurrIndex++;
                }
                else if (MonthNotFound && patt[i] == 'M')
                {
                    MonthNotFound = false;
                    DateOrder[CurrIndex] = 'M';
                    CurrIndex++;
                }
                else if (YearNotFound && patt[i] == 'y')
                {
                    YearNotFound = false;
                    DateOrder[CurrIndex] = 'y';
                    CurrIndex++;
                }
            }
            if (CurrIndex < 3)//if failed to understand right order use a standard order
            {
                DateOrder[0] = 'y';
                DateOrder[1] = 'M';
                DateOrder[2] = 'd';
            }
            return DateOrder;
        }
    }
}
