using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ClaptrapStateAttribute : Attribute
    {
        public Type StateDataType { get; set; }
        public string? ActorTypeCode { get; set; }

        public ClaptrapStateAttribute(Type stateDataType, string actorTypeCode = null)
        {
            StateDataType = stateDataType;
            ActorTypeCode = actorTypeCode;
        }
    }
}