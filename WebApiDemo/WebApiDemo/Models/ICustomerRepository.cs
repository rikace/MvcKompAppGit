using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAllCustomers();

        Customer GetCustomer(int id);

        Customer AddCustomer(Customer item);

        bool RemoveCustomer(int id);

        bool UpdateCustomer(int id, Customer item);
    }
}