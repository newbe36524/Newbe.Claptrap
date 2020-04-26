using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on IClaptrapGrain specify the event data types
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class ClaptrapEventAttribute : Attribute
    {
        public Type EventDataType { get; set; }
        public string? EventTypeCode { get; set; }

        public ClaptrapEventAttribute(Type eventDataType, string eventTypeCode = null)
        {
            EventDataType = eventDataType;
            EventTypeCode = eventTypeCode;
        }
    }
}