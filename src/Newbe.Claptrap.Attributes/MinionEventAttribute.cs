using System;

namespace Newbe.Claptrap.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
    public class MinionEventAttribute : Attribute
    {
        public MinionEventAttribute(string eventType)
        {
            EventType = eventType;
        }

        public string EventType { get; }
    }
}