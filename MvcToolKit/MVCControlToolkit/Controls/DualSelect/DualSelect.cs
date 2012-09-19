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
using System.Collections;
using System.Web.Routing;

namespace MVCControlsToolkit.Controls
{
    public class DualSelect<TModel, TEnumerable, TChoiceItem, TValue, TDisplay>
        where TEnumerable : IEnumerable
    {
        static string DualSelect_Separator = ";;;";
        static string DualSelect_SelectAvial = "_AvialSelect";
        static string DualSelect_SelectSelected = "_SelSelect";
        static string DualSelect_AvailableFilter = "_AvailableFilter";
        static string DualSelect_SelectedFilter = "_SelectedFilter";
        internal HtmlHelper<TModel> CurrHtmlHelper{get;set;}
        private string _prefix;
        private string _fullprefix;
        internal string Prefix{
            get
            {return _prefix;}
            set
            {
                _prefix=value;
                _fullprefix = CurrHtmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(_prefix);              
                controlId = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(_fullprefix,
                     "$.PackedValue"));
            }
        }
        private TEnumerable _value;
        private System.Collections.Generic.HashSet<string> valuesSet = null;
        internal TEnumerable Value 
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                valuesSet = new HashSet<string>();
                foreach (object o in _value as IEnumerable)
                {
                    if (o != null) valuesSet.Add(((TValue)o).ToString());  
                }
            }
        }
        private List<ExtendedSelectListItem> allItems;
        private Dictionary<string, ExtendedSelectListItem> allItemsDictionary = null;
        internal IDictionary<string, object> AttributeExtensions;
        private ChoiceList<TChoiceItem, TValue, TDisplay> _currChoiceList=null;
        internal ChoiceList<TChoiceItem, TValue, TDisplay> CurrChoiceList
        {
            get
            {
                return _currChoiceList;
            }
            set
            {
                _currChoiceList = value;
                allItemsDictionary = new Dictionary<string, ExtendedSelectListItem>();
                allItems = new List<ExtendedSelectListItem>();
                foreach (TChoiceItem x in _currChoiceList.Items)
                {
                    ExtendedSelectListItem currItem =
                        new ExtendedSelectListItem()
                        {
                            Attributes =
                            _currChoiceList.LabelAttributesSelector == null ?
                                null : _currChoiceList.LabelAttributesSelector(x),
                            Value=Convert.ToString(_currChoiceList.ValueSelector(x)),
                            Text= _currChoiceList.DisplaySelector(x).ToString(),
                            Selected=false
                        };
                    allItemsDictionary.Add(
                        _currChoiceList.ValueSelector(x).ToString(),
                        currItem);
                    allItems.Add(currItem);
                }
            }
        }
        private string controlId = string.Empty;
        protected string RenderPackedList()
        {
            PackedList<IEnumerable<TValue>, TValue> displayModel = new PackedList<IEnumerable<TValue>, TValue>();

            displayModel.ImportFromModel(Value, new object[] { DualSelect_Separator });

            StringBuilder sb = new StringBuilder();
            sb.Append(BasicHtmlHelper.RenderDisplayInfo(CurrHtmlHelper,
                typeof(PackedList<IEnumerable<TValue>, TValue>),
                Prefix));
            sb.AppendFormat("<span {2} style='display:none' id='{1}' is-item-control='true' pname='{0}' data-element-type = 'DualSelect' ></span>", _fullprefix, BasicHtmlHelper.IdFromName(_fullprefix), BasicHtmlHelper.GetAttributesString(AttributeExtensions));
            sb.Append(
                CurrHtmlHelper.Hidden(BasicHtmlHelper.AddField(Prefix, "$.Separator"),
                DualSelect_Separator, new RouteValueDictionary{{"data-elementispart", "true"} })).ToString();

            sb.Append(CurrHtmlHelper.Hidden(
                        BasicHtmlHelper.AddField(Prefix, "$.PackedValue"),
                        displayModel.PackedValue, new RouteValueDictionary { { "data-elementispart", "true" } }));
            return sb.ToString();
        }
        public MvcHtmlString SelectedItems(IDictionary<string, object> htmlAttributes=null)
        {
            List<ExtendedSelectListItem> selected = new List<ExtendedSelectListItem>();
            foreach (TValue val in Value)
            {
                if(allItemsDictionary.ContainsKey(val.ToString())) selected.Add(allItemsDictionary[val.ToString()]);
            }
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>() ;
            htmlAttributes["data-elementispart"] = "true";
            return MvcHtmlString.Create
                (
                   RenderPackedList()+
                   CurrHtmlHelper.DropDownbase<TModel,TChoiceItem, TDisplay, TValue> (
                    BasicHtmlHelper.AddField(Prefix, "$.PackedValue") + DualSelect_SelectSelected,
                    new List<TValue>() as IEnumerable<TValue>,
                    selected,
                    htmlAttributes, 
                    true,
                    false).ToString()
                );
        }
        public MvcHtmlString AvailableItems(IDictionary<string, object> htmlAttributes=null)
        {
            List<ExtendedSelectListItem> NotSelected = new List<ExtendedSelectListItem>();
            foreach (ExtendedSelectListItem sel in allItems)
            {
                if (!valuesSet.Contains(sel.Value)) NotSelected.Add(sel);
            }
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            htmlAttributes["data-elementispart"] = "true";
            return MvcHtmlString.Create
                (

                   CurrHtmlHelper.DropDownbase<TModel, TChoiceItem, TDisplay, TValue>(
                    BasicHtmlHelper.AddField(Prefix, "$.PackedValue") + DualSelect_SelectAvial,
                    new List<TValue>() as IEnumerable<TValue>,
                    NotSelected,
                    htmlAttributes,
                    true,
                    false).ToString()
                );
        }
        private static string script = @"
        <script language='javascript' type='text/javascript'>
             DualSelect_FilterInit('{0}', {1});
        </script>
        ";
        public MvcHtmlString AvailableItemsFilter(IDictionary<string, object> htmlAttributes = null)
        {
            return MvcHtmlString.Create( 
                CurrHtmlHelper.TextBox(
                 BasicHtmlHelper.AddField(Prefix, "$.PackedValue") + DualSelect_AvailableFilter,
                 null,
                 htmlAttributes).ToString()+
                    string.Format(script, controlId, "false")
                 );
        }
        public MvcHtmlString SelectedItemsFilter(IDictionary<string, object> htmlAttributes = null)
        {
            return MvcHtmlString.Create( 
                CurrHtmlHelper.TextBox(
                 BasicHtmlHelper.AddField(Prefix, "$.PackedValue")+ DualSelect_SelectedFilter,
                 null,
                 htmlAttributes).ToString()+
                    string.Format(script, controlId, "true")
                 );
        }
        public MvcHtmlString SelectButton(string displayName, ManipulationButtonStyle buttonStyle = ManipulationButtonStyle.Button, IDictionary<string, object> htmlAttributes = null)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>(2);
            htmlAttributes.Add("onclick", string.Format(
                "javascript:return DualSelect_MoveElement('{0}', true)", controlId));
            return BaseButton(displayName, buttonStyle, htmlAttributes);
            
        }
        public MvcHtmlString GoUpButton(string displayName, ManipulationButtonStyle buttonStyle = ManipulationButtonStyle.Button, IDictionary<string, object> htmlAttributes = null)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>(2);
            htmlAttributes.Add("onclick", string.Format(
                "javascript:return DualSelect_Move_Up('{0}', false)", controlId));
            return BaseButton(displayName, buttonStyle, htmlAttributes);

        }
        public MvcHtmlString GoDownButton(string displayName, ManipulationButtonStyle buttonStyle = ManipulationButtonStyle.Button, IDictionary<string, object> htmlAttributes = null)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>(2);
            htmlAttributes.Add("onclick", string.Format(
                "javascript:return DualSelect_Move_Down('{0}', false)", controlId));
            return BaseButton(displayName, buttonStyle, htmlAttributes);

        }
        public MvcHtmlString SelectAllButton(string displayName, ManipulationButtonStyle buttonStyle = ManipulationButtonStyle.Button, IDictionary<string, object> htmlAttributes = null)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>(2);
            htmlAttributes.Add("onclick", string.Format(
                "javascript:return DualSelect_MoveAll('{0}', true)", controlId));
            return BaseButton(displayName, buttonStyle, htmlAttributes);
        }
        public MvcHtmlString UnSelectButton(string displayName, ManipulationButtonStyle buttonStyle = ManipulationButtonStyle.Button, IDictionary<string, object> htmlAttributes = null)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>(2);
            htmlAttributes.Add("onclick", string.Format(
                "javascript:return DualSelect_MoveElement('{0}', false)", controlId));
            return BaseButton(displayName, buttonStyle, htmlAttributes);
        }
        public MvcHtmlString ClearSelectionButton(string displayName, ManipulationButtonStyle buttonStyle = ManipulationButtonStyle.Button, IDictionary<string, object> htmlAttributes = null)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>(2);
            htmlAttributes.Add("onclick", string.Format(
                "javascript:return DualSelect_MoveAll('{0}', false)", controlId));
            return BaseButton(displayName, buttonStyle, htmlAttributes);
        }

        protected MvcHtmlString BaseButton(string displayName,
            ManipulationButtonStyle buttonStyle, IDictionary<string, object> htmlAttributes)
        {
            switch (buttonStyle)
            {
                case ManipulationButtonStyle.Button:
                    return MvcHtmlString.Create
                         (
                         string.Format("<input type=\"button\" value=\"{0}\" {1} />",
                                CurrHtmlHelper.Encode(displayName),
                                BasicHtmlHelper.GetAttributesString(htmlAttributes))
                         );
                case ManipulationButtonStyle.Image:
                    htmlAttributes["src"] = displayName;
                    BasicHtmlHelper.SetDefaultStyle(htmlAttributes, "cursor", "pointer");
                    return MvcHtmlString.Create
                         (
                         string.Format("<img {1} />",
                                displayName,
                                BasicHtmlHelper.GetAttributesString(htmlAttributes))
                         );
                default:
                    htmlAttributes["href"] = "javascript:void(0);";
                    return MvcHtmlString.Create
                         (
                         string.Format("<a {1}>{0}</a>",
                                CurrHtmlHelper.Encode(displayName),
                                BasicHtmlHelper.GetAttributesString(htmlAttributes))
                         );

            }
        }

    }
}
