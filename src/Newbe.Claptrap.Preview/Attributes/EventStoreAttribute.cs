using System;
using Newbe.Claptrap.Preview.EventStore;

namespace Newbe.Claptrap.Preview
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