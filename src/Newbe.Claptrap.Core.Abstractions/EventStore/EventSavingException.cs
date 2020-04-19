using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventStore
{
    public class EventSavingException : Exception
    {
        public EventSavingException(IEvent @event)
            : this($"failed to save event {@event.ActorIdentity.TypeCode} {@event.ActorIdentity.Id} {@event.EventType}",
                @event)
        {
            Event = @event;
        }

        public EventSavingException(string message, IEvent @event) : base(message)
        {
            Event = @event;
        }

        public EventSavingException(string message, Exception innerException, IEvent @event) : base(message,
            innerException)
        {
            Event = @event;
        }

        public IEvent Event { get; set; }
    }
}