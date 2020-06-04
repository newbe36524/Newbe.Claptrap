using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> implementation class to specify the event store options
    /// </summary>
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