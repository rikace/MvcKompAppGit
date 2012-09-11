using System.Data.Entity;
using System.Linq;
using Entities;

namespace MvcKompApp.Models
{
    public class EFProductRepository : IProductRepository
    {
        private NorthwindEntities context = new NorthwindEntities();

        public IQueryable<Product> Products
        {
            get { return context.Products; }
        }

        public void SaveProduct(Product product)
        {
            if (product.ProductID == 0)
            {
                context.Products.AddObject(product);
            }
            context.SaveChanges();
        }

        public void DeleteProduct(Product product)
        {
            context.Products.DeleteObject(product);
            context.SaveChanges();
        }
    }

    public class EFDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}