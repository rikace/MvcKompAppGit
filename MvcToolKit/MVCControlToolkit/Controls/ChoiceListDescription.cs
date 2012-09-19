using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    public class ChoiceListDescription
    {
        public dynamic  Expression{get; set;}
        public dynamic  ChoiceList{get; set;}
        public dynamic HtmlHelper { get; set; }
    }
}
