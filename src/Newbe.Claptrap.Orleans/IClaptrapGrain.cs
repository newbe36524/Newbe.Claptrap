using System;
using Orleans;

namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapGrain : IGrainWithStringKey
    {
    }

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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClaptrapEventHandlerAttribute : Attribute
    {
        public Type ActorStateDataType { get; set; }
        public Type EventDateType { get; set; }
        public string? EventTypeCode { get; set; }

        public ClaptrapEventHandlerAttribute(Type actorStateDataType,
            Type eventDateType,
            string? eventTypeCode = null)
        {
            EventDateType = eventDateType;
            ActorStateDataType = actorStateDataType;
            EventTypeCode = eventTypeCode;
        }
    }
}