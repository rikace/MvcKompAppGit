using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKompApp.Infrastructure
{
    public class Logger : ILog
    {
        public void WriteDbg(string txt)
        {
            System.Diagnostics.Debug.WriteLine(txt);
        }

        public void WriteResponse(string txt)
        {
            System.Diagnostics.Debug.WriteLine(txt);
        }
    }
}