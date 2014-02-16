using System;
using System.Threading;
using Common.Framework;
using MvcNotes.Infrastructure;
using MvcNotes.Logging;

namespace MvcNotes.Events
{
    public class PresidentEventHandlers :
        IEventHandler<PresidentAddedEvent>
    {
        public void Handle(PresidentAddedEvent handle)
        {
           
        }
    }
}

     