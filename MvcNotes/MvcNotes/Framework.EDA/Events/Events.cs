using System;
using Domain.Entities;
using Entity;
using President = Domain.Entities.President;

namespace MvcNotes.Events
{
    public class PresidentAddedEvent : Event
    {
        public PresidentAddedEvent(Entity.President president, Guid? id = null)
        {
            President = President;
            Id = id ?? Guid.NewGuid();
        }

        public President President { get; private set; }
    }
}