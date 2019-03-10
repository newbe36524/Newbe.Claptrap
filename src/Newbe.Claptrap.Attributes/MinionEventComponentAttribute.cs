using System;

namespace Newbe.Claptrap.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MinionEventComponentAttribute : Attribute
    {
        public string MinionCatalog { get; }
        public string Catalog { get; }
        public string EventType { get; }

        public MinionEventComponentAttribute(string minionCatalog, string catalog, string eventType)
        {
            MinionCatalog = minionCatalog;
            Catalog = catalog;
            EventType = eventType;
        }
    }
}