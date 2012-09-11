using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcKompApp.WorkerServices.Abstraction;

namespace MvcKompApp.WorkerServices.Customer
{
    public class CustomerWorkerServices : ICustomerWorkerServices
    {
        private static List<Models.Customer> custmers = new List<MvcKompApp.Models.Customer>
                {
                    new MvcKompApp.Models.Customer{ FirstName="Bugghina", Age=5, LastName ="Terrell", Id=0},
                    new MvcKompApp.Models.Customer{ FirstName="Riccardo", Age=38, LastName ="Terrell", Id=1}
                };


        public ViewModels.ListCustmersViewModel FindAllCustomers()
        {
            return new ViewModels.ListCustmersViewModel
            {
                 Customers =custmers
            };
        }


        public Models.Customer FindCustmer(int id)
        {
            return custmers.FirstOrDefault(c => c.Id == id);
        }
    }
}