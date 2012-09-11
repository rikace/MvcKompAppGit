using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcKompApp.Models;

namespace MvcKompApp.ViewModels
{
    public class ListCustmersViewModel
    {
        public IList<Customer> Customers { get; set; }
    }
}