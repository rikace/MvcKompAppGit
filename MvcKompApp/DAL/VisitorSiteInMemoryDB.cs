using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcKompApp.Models;

namespace MvcKompApp.DAL
{
    public class VisitorSiteInMemoryDB
    {
        public static List<VisitorSiteModel> GetVisitors()
        {
            return new List<VisitorSiteModel>
            {
                new VisitorSiteModel{ IpAddress="169.123.15.1", Latitude ="80", Longitude ="78.32"},
                new VisitorSiteModel{ IpAddress="245.12.157.1", Latitude ="73.565", Longitude ="14.32"},
                new VisitorSiteModel{ IpAddress="16.13.1.1", Latitude ="75.1", Longitude ="35.41"},

            };
        }
    }
}