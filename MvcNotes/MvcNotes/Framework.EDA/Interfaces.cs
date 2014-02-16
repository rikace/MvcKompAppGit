using System;
using System.Collections.Generic;

namespace Common.Framework
{
    public interface IMessage
    {
        Guid Id { get; }
    }

    public interface IEventStore<TEvent> where TEvent : class
    {
        void SaveEvent(Guid id, TEvent @event);
        void SaveEvents(Guid id, IEnumerable<TEvent> eventsn);
        List<TEvent> GetEvents(Guid id);
    }

    public interface ICommand : IMessage
    {
    }

    public interface IEvent : IMessage
    {
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void Execute(TCommand command);
    }

    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        void Handle(TEvent handle);
    }

    public interface ICommandDispatcher
    {
        void Dispatch<TCommand>(TCommand command) where TCommand : ICommand;
    }
    
    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }

    public interface ISubscriber
    {
        void RegisterHandler<T>(Action<T> handler) where T : IMessage;
    }
}
