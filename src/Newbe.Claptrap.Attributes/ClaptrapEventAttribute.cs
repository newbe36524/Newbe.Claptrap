using System;

namespace Newbe.Claptrap
{
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