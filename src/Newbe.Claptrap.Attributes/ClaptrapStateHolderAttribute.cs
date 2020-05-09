using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ClaptrapStateHolderAttribute : Attribute
    {
        public Type StateHolderFactory { get; }

        public ClaptrapStateHolderAttribute(Type stateHolderFactory)
        {
            StateHolderFactory = stateHolderFactory;
        }
    }
}