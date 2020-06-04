using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> implementation class to specify the state store options
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ClaptrapStateStoreAttribute : Attribute
    {
        public Type StateSaverFactoryType { get; }
        public Type StateLoaderFactoryType { get; }

        public ClaptrapStateStoreAttribute(Type stateSaverFactoryType, Type stateLoaderFactoryType)
        {
            StateSaverFactoryType = stateSaverFactoryType;
            StateLoaderFactoryType = stateLoaderFactoryType;
        }
    }
}