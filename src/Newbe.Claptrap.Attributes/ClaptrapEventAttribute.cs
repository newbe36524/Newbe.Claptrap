using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> specify the event 
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class ClaptrapEventAttribute : Attribute
    {
        public Type EventDataType { get; set; }
        public string EventTypeCode { get; set; }

        public ClaptrapEventAttribute(Type eventDataType, string eventTypeCode)
        {
            EventDataType = eventDataType;
            EventTypeCode = eventTypeCode;
        }
    }
}