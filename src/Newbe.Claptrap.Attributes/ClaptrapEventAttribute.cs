using System;

namespace Newbe.Claptrap.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ClaptrapEventAttribute : Attribute
    {
        public string EventType { get; }
        public Type EventDataType { get; }

        public ClaptrapEventAttribute(string eventType, Type eventDataType)
        {
            EventType = eventType;
            EventDataType = eventDataType;
        }
    }
}