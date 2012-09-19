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
using System.Globalization;



namespace MVCControlsToolkit.Core
{
    public class ClientChoiceItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
    public class ClientChoiceGroup : ClientChoiceItem
    {
        public IEnumerable<ClientChoiceItem> Group {get; set;}
    }
    public class ChoiceList<TItem, TValue, TDisplay>
    {
        private IEnumerable<TItem> _items;
        internal object origin;
        internal bool isInClientBlock;
        public IEnumerable<TItem> Items
        {
            get
            {
                return _items;
            }
        }
        private Func<TItem, TValue> _valueSelector;
        internal Expression<Func<TItem, TValue>> _evalueSelector;
        public Func<TItem, TValue> ValueSelector
        {
            get
            {
                return _valueSelector;
            }
        }

        private Func<TItem, TDisplay> _displaySelector;
        internal Expression<Func<TItem, TDisplay>> _edisplaySelector;
        public Func<TItem, TDisplay> DisplaySelector
        {
            get
            {
                return _displaySelector;
            }
        }

        private Func<TItem, object> _labelAttributesSelector;

        public Func<TItem, object> LabelAttributesSelector
        {
            get
            {
                return _labelAttributesSelector;
            }
        }

        private Func<TItem, object> _displayAttributesSelector;

        public Func<TItem, object> DisplayAttributesSelector
        {
            get
            {
                return _displayAttributesSelector;
            }
        }

        private bool _usePrompt;
        public bool UsePrompt
        {
            get
            {
                return _usePrompt;
            }
        }
        private string _overridePrompt;
        public string OverridePrompt
        {
            get
            {
                return _overridePrompt;
            }
        }
        public ChoiceList(IEnumerable<TItem> items, 
            Expression<Func<TItem, TValue>> valueSelector,
            Expression<Func<TItem, TDisplay>> displaySelector,
            Func<TItem, object> valueAttributesSelector=null,
            Func<TItem, object> labelAttributesSelector=null,
            bool usePrompt = true,
            string overridePrompt=null)
        {
            _items = items;
            _evalueSelector = valueSelector;
            _edisplaySelector = displaySelector;
            _valueSelector = valueSelector.Compile();
            _displaySelector = displaySelector.Compile();
            _displayAttributesSelector = valueAttributesSelector;
            _labelAttributesSelector = labelAttributesSelector;
            _usePrompt = usePrompt;
            _overridePrompt = overridePrompt;
        }
        public virtual object PrepareForJson()
        {
            return Items.Select(m => new ClientChoiceItem
            {
                Value = Convert.ToString(ValueSelector(m), CultureInfo.CurrentCulture),
                Text = Convert.ToString(DisplaySelector(m), CultureInfo.CurrentCulture)
            });
        }
    }
    public interface IGroupedChoiceList
    {
        dynamic GroupValueSelector { get; }
        dynamic GroupDisplaySelector { get; }
        dynamic GroupAttributesSelector { get; }
    }
    public class GroupedChoiceList<TItem, TValue, TDisplay, TGroup, TDisplayGroup> : ChoiceList<TItem, TValue, TDisplay>, IGroupedChoiceList
    {
        private dynamic _groupAttributesSelector;
        public dynamic GroupAttributesSelector
        {
            get
            {
                return _groupAttributesSelector;
            }
        }

        private dynamic _groupValueSelector;

        public dynamic GroupValueSelector
        {
            get
            {
                return _groupValueSelector;
            }
        }
        private dynamic _groupDisplaySelector;

        public dynamic GroupDisplaySelector
        {
            get
            {
                return _groupDisplaySelector;
            }
        }
        public GroupedChoiceList(IEnumerable<TItem> items,
            Expression<Func<TItem, TValue>> valueSelector,
            Expression<Func<TItem, TDisplay>> displaySelector,
            Func<TItem, TGroup> groupValueSelector,
            Func<TItem, TDisplayGroup> groupDisplaySelector,
            Func<TItem, object> groupAttributesSelector = null, 
            Func<TItem, object> valueAttributesSelector = null,
            Func<TItem, object> labelAttributesSelector = null,
            bool usePrompt = true,
            string overridePrompt=null
            
            ):
            base(items, valueSelector, displaySelector, 
                valueAttributesSelector, labelAttributesSelector, usePrompt, overridePrompt)
        {
            _groupValueSelector = groupValueSelector;
            _groupDisplaySelector = groupDisplaySelector;
            _groupAttributesSelector = groupAttributesSelector;
        }
        public override object PrepareForJson()
        {
            var res = new List<ClientChoiceItem>();
            var allGroups = Items.GroupBy(m => GroupValueSelector(m));
            foreach (var group in allGroups)
            {
                var groupLabel = Convert.ToString(group.Select(it => GroupDisplaySelector(it)).FirstOrDefault(), CultureInfo.CurrentCulture);
                if (group.Key != null && !string.IsNullOrWhiteSpace(groupLabel))
                {
                    res.Add(new ClientChoiceGroup
                        {
                            Value = Convert.ToString(group.Key, CultureInfo.CurrentCulture),
                            Text = groupLabel,
                            Group = group.Select(it => new ClientChoiceItem { Value = Convert.ToString(ValueSelector(it), CultureInfo.CurrentCulture), Text = Convert.ToString(DisplaySelector(it), CultureInfo.CurrentCulture) })
                        });
                }
                else
                {
                    res.AddRange(group.Select(it => new ClientChoiceItem { Value = Convert.ToString(ValueSelector(it), CultureInfo.CurrentCulture), Text = Convert.ToString(DisplaySelector(it), CultureInfo.CurrentCulture) }));
                }
            }
            return res;
        }
    }
}
