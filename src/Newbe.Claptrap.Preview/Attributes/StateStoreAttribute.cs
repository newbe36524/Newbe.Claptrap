using System;
using Newbe.Claptrap.Preview.EventStore;

namespace Newbe.Claptrap.Preview
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