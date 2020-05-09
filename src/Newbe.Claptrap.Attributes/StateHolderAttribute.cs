using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class StateHolderAttribute : Attribute
    {
        public Type StateHolderFactory { get; }

        public StateHolderAttribute(Type stateHolderFactory)
        {
            StateHolderFactory = stateHolderFactory;
        }
    }
}