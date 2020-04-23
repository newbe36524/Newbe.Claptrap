using System;

namespace Newbe.Claptrap.Orleans
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClaptrapEventHandlerAttribute : Attribute
    {
        public Type ActorStateDataType { get; set; }
        public Type EventDateType { get; set; }
        public string? EventTypeCode { get; set; }

        public ClaptrapEventHandlerAttribute(Type actorStateDataType,
            Type eventDateType,
            string? eventTypeCode = null)
        {
            EventDateType = eventDateType;
            ActorStateDataType = actorStateDataType;
            EventTypeCode = eventTypeCode;
        }
    }
}