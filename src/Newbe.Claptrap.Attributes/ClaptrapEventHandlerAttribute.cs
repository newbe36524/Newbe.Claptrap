using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> implementation class to specify the event handler types
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClaptrapEventHandlerAttribute : Attribute
    {
        public Type EventHandlerType { get; set; }
        public string EventTypeCode { get; set; }

        public ClaptrapEventHandlerAttribute(Type eventHandlerType, string eventTypeCode)
        {
            EventHandlerType = eventHandlerType;
            EventTypeCode = eventTypeCode;
        }
    }
}