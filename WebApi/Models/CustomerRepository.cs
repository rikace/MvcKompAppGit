using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebApi.Database;

namespace WebApi.Models
{
    public class CustomerRepository : ICustomerRepository
    {
        private CustomersDbContext context;

        public CustomerRepository()
        {
            context = new CustomersDbContext();
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            try
            {
                return context.Customers.ToList();
            }
            catch
            {
                throw;
            }
        }

        public Customer GetCustomer(int id)
        {
            try
            {
                return context.Customers.SingleOrDefault(c => c.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public Customer AddCustomer(Customer item)
        {
            try
            {
                context.Customers.Add(item);
                context.SaveChanges();
                return item;
            }
            catch
            {
                throw;
            }
        }

        public bool RemoveCustomer(int id)
        {
            try
            {
                var contact = context.Customers.SingleOrDefault(c => c.Id == id);
                if (contact == null)
                    throw new Exception(string.Format("Contact with id: '{0}' not found", id.ToString()));

                context.Customers.Remove(contact);
                context.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public bool UpdateCustomer(int id, Customer item)
        {
            try
            {
                var customer = context.Customers.SingleOrDefault(c => c.Id == id);

                if (customer == null)
                    throw new Exception(string.Format("Customer with id: '{0}' not found", id.ToString()));

                customer.Name = item.Name;
                customer.Email = item.Email;
                customer.Phone = item.Phone;

                context.Entry(customer).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
    }

}