using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> implementation class to specify the state data initialization factory
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapStateInitialFactoryHandlerAttribute : Attribute
    {
        public Type? StateInitialFactoryHandlerType { get; }

        public ClaptrapStateInitialFactoryHandlerAttribute(Type? stateInitialFactoryHandlerType = null)
        {
            StateInitialFactoryHandlerType = stateInitialFactoryHandlerType;
        }
    }
}