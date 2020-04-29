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
        public Type EventDataType { get; set; }

        public ClaptrapEventHandlerAttribute(Type eventHandlerType, Type eventDataType)
        {
            EventHandlerType = eventHandlerType;
            EventDataType = eventDataType;
        }
    }
}