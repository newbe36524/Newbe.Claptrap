using System;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Exceptions
{
    public class EventSavingException : Exception
    {
        public EventSavingException(IEvent @event)
            : this(
                $"failed to save event {@event.ClaptrapIdentity.TypeCode} {@event.ClaptrapIdentity.Id} {@event.EventTypeCode}",
                @event)
        {
            Event = @event;
        }

        public EventSavingException(string message, IEvent @event) : base(message)
        {
            Event = @event;
        }

        public EventSavingException(Exception innerException, IEvent @event) : base(
            $"failed to save event {@event.ClaptrapIdentity.TypeCode} {@event.ClaptrapIdentity.Id} {@event.EventTypeCode}",
            innerException)
        {
            Event = @event;
        }

        public IEvent Event { get; set; }
    }
}