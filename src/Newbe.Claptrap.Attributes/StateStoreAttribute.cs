using System;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class StateStoreAttribute : Attribute
    {
        public StateStoreProvider StateStoreProvider { get; }

        public StateStoreAttribute(StateStoreProvider stateStoreProvider)
        {
            StateStoreProvider = stateStoreProvider;
        }
    }
}