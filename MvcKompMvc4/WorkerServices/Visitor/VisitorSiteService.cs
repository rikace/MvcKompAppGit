using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcKompApp.DAL;

namespace MvcKompApp.WorkerServices
{
    public class VisitorSiteService : IVisitorSiteService
    {

        public List<Models.VisitorSiteModel> GetVisitors()
        {
            return VisitorSiteInMemoryDB.GetVisitors();
        }

        public Models.VisitorSiteModel FindVisitor(string ipAddress)
        {
            throw new NotImplementedException();
        }
    }
}