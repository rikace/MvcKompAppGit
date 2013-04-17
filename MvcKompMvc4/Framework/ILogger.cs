using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKompApp.Framework
{
    public interface ILogger  
    {                         
        void Log(string message);
    }
}