using System;

namespace Newbe.Claptrap.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClaptrapEventComponentAttribute : Attribute
    {
        public string Catalog { get; }
        public string EventType { get; }

        public ClaptrapEventComponentAttribute(string catalog, string eventType)
        {
            Catalog = catalog;
            EventType = eventType;
        }
    }
}