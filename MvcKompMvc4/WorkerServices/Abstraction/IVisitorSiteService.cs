using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcKompApp.Models;

namespace MvcKompApp.WorkerServices
{
    public interface IVisitorSiteService
    {
        List<VisitorSiteModel> GetVisitors();
        VisitorSiteModel FindVisitor(string ipAddress);
    }
}