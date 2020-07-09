using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> implementation class to specify the state holder
    /// </summary>
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