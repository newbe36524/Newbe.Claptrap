using System;

namespace Newbe.Claptrap.Preview.Attributes
{
    /// <summary>
    /// Mark on IClaptrapGrain implementation class to specify the event handler types
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