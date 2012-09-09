using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKompApp.Infrastructure
{
    public interface ILog
    {
         void WriteDbg(string txt);
         void WriteResponse(string txt);
    }
}