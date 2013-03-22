using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApi.Models;

namespace WebApi.Database
{
    public class CustomersDbContext : DbContext
    {
        public CustomersDbContext()
           // : base("name=CustomerDbContext")
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}