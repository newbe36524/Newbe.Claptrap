using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class StateStoreAttribute : Attribute
    {
        public Type StateSaverFactoryType { get; }
        public Type StateLoaderFactoryType { get; }

        public StateStoreAttribute(Type stateSaverFactoryType, Type stateLoaderFactoryType)
        {
            StateSaverFactoryType = stateSaverFactoryType;
            StateLoaderFactoryType = stateLoaderFactoryType;
        }
    }
}