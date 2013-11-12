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

    public class DatabaseCustomersDbContextInit : CreateDatabaseIfNotExists<CustomersDbContext>
    {
        protected override void Seed(CustomersDbContext context)
        {
            context.Customers.Add(new Customer { Name ="Riccardo", Email="s.riccardo@hotmail.com", Phone="30457895672" });
            base.Seed(context);
        }
    }
}