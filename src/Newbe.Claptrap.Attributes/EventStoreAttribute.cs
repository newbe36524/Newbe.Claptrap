using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class EventStoreAttribute : Attribute
    {
        public Type EventSaverFactoryType { get; }
        public Type EventLoaderFactoryType { get; }

        public EventStoreAttribute(
            Type eventSaverFactoryType,
            Type eventLoaderFactoryType)
        {
            EventSaverFactoryType = eventSaverFactoryType;
            EventLoaderFactoryType = eventLoaderFactoryType;
        }
    }
}