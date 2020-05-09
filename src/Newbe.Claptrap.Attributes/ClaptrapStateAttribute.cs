using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on IClaptrapGrain specify the state data class
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ClaptrapStateAttribute : Attribute
    {
        public Type StateDataType { get; set; }
        public string ActorTypeCode { get; set; }

        public ClaptrapStateAttribute(Type stateDataType, string actorTypeCode)
        {
            StateDataType = stateDataType;
            ActorTypeCode = actorTypeCode;
        }
    }
}