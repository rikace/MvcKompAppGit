using System.ComponentModel.Composition;
using System.Data.Entity;
using Entity;
using Domain.Entities;
using President = Entity.President;

namespace Domain.DataAccess
{
    [Export(typeof (DbContext))]
    public class PresidentContext : DbContext
    {
        public PresidentContext()
            : base("name=PresidentContext")
        {
        }

        public DbSet<President> Presidents { get; set; }
    }
}