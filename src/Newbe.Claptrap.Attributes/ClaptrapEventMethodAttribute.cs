using System;

namespace Newbe.Claptrap.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ClaptrapEventMethodAttribute : ClaptrapEventAttribute
    {
        public ClaptrapEventMethodAttribute(string eventType, Type eventDataType)
            : base(eventType, eventDataType)
        {
        }
    }
}