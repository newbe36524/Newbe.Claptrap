using System;

namespace Newbe.Claptrap.Preview.Core
{
    public class EventHandlerNotFoundException : Exception
    {
        public string ActorTypeCode { get; set; }
        public string EventTypeCode { get; set; }

        public EventHandlerNotFoundException(string actorTypeCode, string eventTypeCode)
            : this($"EventHandler not found for {actorTypeCode} {eventTypeCode}. please make sure you have registered",
                actorTypeCode,
                eventTypeCode)
        {
        }

        public EventHandlerNotFoundException(string message, string actorTypeCode, string eventTypeCode) : base(message)
        {
            ActorTypeCode = actorTypeCode;
            EventTypeCode = eventTypeCode;
        }

        public EventHandlerNotFoundException(string message, Exception innerException, string actorTypeCode,
            string eventTypeCode) : base(message, innerException)
        {
            ActorTypeCode = actorTypeCode;
            EventTypeCode = eventTypeCode;
        }
    }
}