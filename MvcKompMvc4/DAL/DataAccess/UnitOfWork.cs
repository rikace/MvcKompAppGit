using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace MvcKompApp.DAL.DataAccess
{
    public class UnitOfWork
    {
        public UnitOfWork(DataContext context)
        {
            //Contract.Requires<ArgumentNullException>(context != null);

            Context = context;
        }

        protected DataContext Context { get; private set; }

        public virtual void Commit()
        {
            if (Context.ChangeTracker.Entries().Any(HasChanged))
            {
                Context.SaveChanges();
            }
        }

        private static bool HasChanged(DbEntityEntry entity)
        {
            return IsState(entity, EntityState.Added) ||
                   IsState(entity, EntityState.Deleted) ||
                   IsState(entity, EntityState.Modified);
        }

        private static bool IsState(DbEntityEntry entity, EntityState state)
        {
            return (entity.State & state) == state;
        }
    }
}