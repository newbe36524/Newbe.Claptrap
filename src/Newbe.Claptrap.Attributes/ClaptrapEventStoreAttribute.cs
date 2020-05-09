using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ClaptrapEventStoreAttribute : Attribute
    {
        public Type EventSaverFactoryType { get; }
        public Type EventLoaderFactoryType { get; }

        public ClaptrapEventStoreAttribute(
            Type eventSaverFactoryType,
            Type eventLoaderFactoryType)
        {
            EventSaverFactoryType = eventSaverFactoryType;
            EventLoaderFactoryType = eventLoaderFactoryType;
        }
    }
}