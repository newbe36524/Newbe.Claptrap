using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on IClaptrapGrain implementation class to specify the state initialization factory
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