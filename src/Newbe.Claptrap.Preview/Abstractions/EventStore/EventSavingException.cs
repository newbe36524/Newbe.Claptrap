using System;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.EventStore
{
    public class EventSavingException : Exception
    {
        public EventSavingException(IEvent @event)
            : this($"failed to save event {@event.ActorIdentity.TypeCode} {@event.ActorIdentity.Id} {@event.EventTypeCode}",
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