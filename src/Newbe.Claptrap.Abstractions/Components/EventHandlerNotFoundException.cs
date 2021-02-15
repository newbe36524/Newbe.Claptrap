using System;

namespace Newbe.Claptrap
{
    public class EventHandlerNotFoundException : Exception
    {
        public string ClaptrapTypeCode { get; set; }
        public string EventTypeCode { get; set; }

        public EventHandlerNotFoundException(string claptrapTypeCode, string eventTypeCode)
            : this(
                $"EventHandler not found for {claptrapTypeCode} {eventTypeCode}. please make sure you have registered",
                claptrapTypeCode,
                eventTypeCode)
        {
        }

        public EventHandlerNotFoundException(string message, string claptrapTypeCode, string eventTypeCode) :
            base(message)
        {
            ClaptrapTypeCode = claptrapTypeCode;
            EventTypeCode = eventTypeCode;
        }

        public EventHandlerNotFoundException(string message, Exception innerException, string claptrapTypeCode,
            string eventTypeCode) : base(message, innerException)
        {
            ClaptrapTypeCode = claptrapTypeCode;
            EventTypeCode = eventTypeCode;
        }
    }
}