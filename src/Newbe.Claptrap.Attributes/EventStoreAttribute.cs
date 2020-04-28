using System;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class EventStoreAttribute : Attribute
    {
        public EventStoreProvider EventStoreProvider { get; }

        public EventStoreAttribute(EventStoreProvider eventStoreProvider)
        {
            EventStoreProvider = eventStoreProvider;
        }
    }
}