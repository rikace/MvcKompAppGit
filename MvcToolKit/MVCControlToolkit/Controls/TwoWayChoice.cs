using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    public class TwoWayChoice<TItem>:IDisplayModel
    {
        public TItem Choice1 { get; set; }
        public TItem Choice2 { get; set; }
        public bool IsChoice2 { get; set; }



        public object ExportToModel(Type TargetType, params object[] context)
        {
            if (context != null && context.Length > 1)
            {
                IDictionary dict = (context[1] as IDictionary);
                if (dict != null) dict[context[0] as string] = IsChoice2;

            }
            if (IsChoice2) return Choice2;
            else return Choice1;
        }

        public void ImportFromModel(object model, params object[] context)
        {
            if (model == null) return;
            Choice1 = (TItem)model;
            Choice2 = (TItem)model;
        }
    }
}
