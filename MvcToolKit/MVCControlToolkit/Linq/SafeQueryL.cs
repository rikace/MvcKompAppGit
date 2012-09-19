using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using MVCControlsToolkit.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVCControlsToolkit.Linq
{
    public enum FilterCondition { None = 0, Equal = 1, NotEqual = 2, LessThan = 4, LessThanOrEqual = 8, GreaterThan = 16, GreaterThanOrEqual = 32, StartsWith = 64, EndsWith = 128, Contains = 256, IsContainedIn = 512 }

}
