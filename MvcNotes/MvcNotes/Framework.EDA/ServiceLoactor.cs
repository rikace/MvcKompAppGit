using System;
using System.Threading;
using Common.Framework;
using MvcNotes.Events;
using MvcNotes.Framework;
using MvcNotes.Framework.Events;

namespace MvcNotes
{
    public sealed class ServiceLocator
    {
        private static readonly Lazy<BusObservable> commandBus =
            new Lazy<BusObservable>(() => new BusObservable(),
                LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<IEventStore<Event>> eventStore =
            new Lazy<IEventStore<Event>>(() => new EventStore(EventBus),
                LazyThreadSafetyMode.PublicationOnly);

        static ServiceLocator()
        {
        }

        public static ICommandDispatcher CommandBus
        {
            get { return commandBus.Value; }
        }

        public static ISubscriber Subscriber
        {
            get { return commandBus.Value; }
        }

        public static IEventPublisher EventBus
        {
            get { return commandBus.Value; }
        }

        public static IEventStore<Event> EventStore
        {
            get { return eventStore.Value; }
        }
    }
}