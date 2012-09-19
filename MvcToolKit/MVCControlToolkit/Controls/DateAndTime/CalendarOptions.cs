using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace MVCControlsToolkit.Controls
{
    public enum CalendarOptionsShowOn {focus, button, both};
    public class CalendarOptions
    {
        private string getShowOn(CalendarOptionsShowOn showOn)
        {
            switch (showOn)
            {
                case CalendarOptionsShowOn.focus: return "focus";
                case CalendarOptionsShowOn.button: return "button";
                default: return "both";
            }
        }
        public CalendarOptions()
        {
            AutoSize = false;
            ButtonImage = string.Empty;
            ButtonImageOnly = false;
            ButtonText = "...";
            ChangeMonth = false;
            ChangeYear = false;
            Duration = -1;
            GoToCurrent = false;
            HideIfNoPrevNext = false;
            NavigationAsDateFormat = false;
            NumberOfMonths = 1;
            SelectOtherMonths = false;
            ShowAnim = "show";
            ShowOn = CalendarOptionsShowOn.focus;
            ShowButtonPanel = false;
            ShowCurrentAtPos = 0;
            ShowOtherMonths = false;
            ShowWeek = false;
            StepMonths = 1;
            YearRange = @"c-10:c+10";
        }
        public bool AutoSize { get; set; }
        public string ButtonImage { get; set; }
        public bool ButtonImageOnly { get; set; }
        public string ButtonText { get; set; }
        public bool ChangeMonth { get; set; }
        public bool ChangeYear { get; set; }
        public int Duration { get; set; }
        public bool GoToCurrent { get; set; }
        public bool HideIfNoPrevNext { get; set; }
        public bool NavigationAsDateFormat { get; set; }
        public int NumberOfMonths { get; set; }     
        public bool SelectOtherMonths { get; set; }
        public string ShowAnim { get; set; }
        public CalendarOptionsShowOn ShowOn { get; set; }
        public bool ShowButtonPanel { get; set; }
        public int ShowCurrentAtPos { get; set; }
        public bool ShowOtherMonths { get; set; }
        public bool ShowWeek { get; set; }
        public string YearRange { get; set; }
        public int StepMonths { get; set; }
        public string DateFormat { get; set; }

        private void AddBool(StringBuilder sb, string name, bool value)
        {
            sb.Append(", ");
            sb.Append(name);
            sb.Append(": ");
            sb.Append(value ? "true": "false");
        }
        private void AddInt(StringBuilder sb, string name, int value)
        {
            sb.Append(", ");
            sb.Append(name);
            sb.Append(": ");
            sb.Append(value.ToString());
        }
        private void AddString(StringBuilder sb, string name, string value)
        {
            sb.Append(", ");
            sb.Append(name);
            if (name == null)
            {
                sb.Append(": null");
            }
            else
            {
                sb.Append(": '");
                sb.Append(value);
                sb.Append("'");
            }
        }
        internal string Render(string clientId, bool inLine=true, bool forTypedInput=false)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{ ");
            if (forTypedInput)
            {
                sb.Append("onSelect: function(dateText, inst) {MvcControlsToolkit_TypedInput_Load(dateText, '");
            }
            else
            {
                if (inLine)
                    sb.Append("onSelect: function(dateText, inst) {DateTimeInput_UpdateFromCalendar(dateText, '");
                else
                    sb.Append("onClose: function(dateText, inst) {DateTimeInput_UpdateFromCalendar(dateText, '");
            }
            sb.Append(clientId);
            sb.Append("');}");
            
            AddBool(sb, "autoSize", AutoSize);
            AddString(sb, "buttonImage", ButtonImage);
            AddBool(sb, "buttonImageOnly", ButtonImageOnly);
            AddString(sb, "buttonText", ButtonText);
            AddBool(sb, "changeMonth", ChangeMonth);
            AddBool(sb, "changeYear", ChangeYear);
            if (Duration >= 0) AddInt(sb, "duration", Duration);
            AddBool(sb, "gotoCurrent", GoToCurrent);
            AddBool(sb, "hideIfNoPrevNext", HideIfNoPrevNext);
            AddBool(sb, "navigationAsDateFormat", NavigationAsDateFormat);
            AddInt(sb, "numberOfMonths", NumberOfMonths);
            AddBool(sb, "selectOtherMonths", SelectOtherMonths);
            AddString(sb, "showAnim", ShowAnim);
            AddString(sb, "showOn", getShowOn(ShowOn));
            AddBool(sb, "showButtonPanel", ShowButtonPanel);
            AddInt(sb, "showCurrentAtPos", ShowCurrentAtPos);
            AddBool(sb, "showOtherMonths", ShowOtherMonths);
            AddBool(sb, "showWeek", ShowWeek);
            AddString(sb, "yearRange", YearRange);
            AddInt(sb, "stepMonths", StepMonths);
            if (DateFormat != null) AddString(sb, "dateFormat", DateFormat);
            sb.Append(" }");
            return sb.ToString();
        }

    }
    
}
