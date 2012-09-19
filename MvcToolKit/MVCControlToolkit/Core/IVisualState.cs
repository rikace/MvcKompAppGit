using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCControlsToolkit.Core
{
    public interface IVisualState
    {
        IDictionary Store { set; }
        string ModelName { set; }
    }
    public interface IHandleUpdateIndex
    {
        int Index { get; set; }
        ModelStateDictionary ModelState { get; set; } 
    }
}
