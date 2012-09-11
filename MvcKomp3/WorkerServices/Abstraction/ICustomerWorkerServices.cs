using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcKompApp.ViewModels;

namespace MvcKompApp.WorkerServices.Abstraction
{
    public interface ICustomerWorkerServices
    {
        ListCustmersViewModel FindAllCustomers();

        MvcKompApp.Models.Customer FindCustmer(int id);
    }
}