using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapStateInitialFactoryHandlerAttribute : Attribute
    {
        public Type StateInitialFactoryHandlerType { get; }

        public ClaptrapStateInitialFactoryHandlerAttribute(Type stateInitialFactoryHandlerType = null)
        {
            StateInitialFactoryHandlerType = stateInitialFactoryHandlerType;
        }
    }
}