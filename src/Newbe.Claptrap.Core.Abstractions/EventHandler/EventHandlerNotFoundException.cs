using System;

namespace Newbe.Claptrap.EventHandler
{
    public class EventHandlerNotFoundException : Exception
    {
        public string ActorTypeCode { get; set; }
        public string EventType { get; set; }

        public EventHandlerNotFoundException(string actorTypeCode, string eventType)
            : this($"EventHandler not found for {actorTypeCode} {eventType}. please make sure you have registered",
                actorTypeCode,
                eventType)
        {
        }

        public EventHandlerNotFoundException(string message, string actorTypeCode, string eventType) : base(message)
        {
            ActorTypeCode = actorTypeCode;
            EventType = eventType;
        }

        public EventHandlerNotFoundException(string message, Exception innerException, string actorTypeCode,
            string eventType) : base(message, innerException)
        {
            ActorTypeCode = actorTypeCode;
            EventType = eventType;
        }
    }
}