using System;

namespace Newbe.Claptrap.Orleans
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClaptrapStateAttribute : Attribute
    {
        public Type StateDataType { get; set; }
        public Type? InitialStateDataFactoryHandlerType { get; set; }
        public string? ActorTypeCode { get; set; }

        public ClaptrapStateAttribute(
            Type stateDataType,
            Type? initialStateDataFactoryHandlerType = null,
            string? actorTypeCode = null)
        {
            StateDataType = stateDataType;
            ActorTypeCode = actorTypeCode;
            InitialStateDataFactoryHandlerType = initialStateDataFactoryHandlerType;
        }
    }
}