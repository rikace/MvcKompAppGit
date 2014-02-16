using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Common.Framework;
using Entity;
using Domain.Common;
using Domain.Entities;
using President = Entity.President;

namespace Domain.DataAccess
{
    public interface IPresidentUnitOfWork : IUnitOfWork
    {
        IRepository<President> Presidents { get; }
       
        void Commit();

        void Commit<TEvent>(Guid id, IEventStore<TEvent> eventStore) where TEvent : class;
    }

    [Export(typeof (IPresidentUnitOfWork))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PresidentUnitOfWork : IPresidentUnitOfWork
    {
        private ContextRepository<President> _presidents;

        private PresidentContext ctx;

        public PresidentUnitOfWork()
        {
            ctx = new PresidentContext();
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public void Dispose()
        {
            ctx.Dispose();
            ctx = null;
        }

        public IRepository<President> Presidents
        {
            get { return _presidents ?? (_presidents = new ContextRepository<President>(ctx)); }
        }

        public void Commit<TEvent>(Guid id, IEventStore<TEvent> eventStore) where TEvent : class
        {
            List<TEvent> events = eventStore.GetEvents(id);
            eventStore.SaveEvents(id, events);

            ctx.SaveChanges();
        }

        public void Commit()
        {
            ctx.SaveChanges();
        }

        public void Rollback()
        {
            // Not Implemented
        }
    }
}