using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Orleans
{
    public static class ClaptrapGrainExtensions
    {
        public static DataEvent CreateEvent<TStateData, TEventDataType>(
            this IClaptrapGrain<TStateData> claptrapGrain,
            TEventDataType eventData)
            where TStateData : IStateData
            where TEventDataType : IEventData
        {
            return CreateEvent(claptrapGrain, eventData, Guid.NewGuid().ToString());
        }

        public static DataEvent CreateEvent<TStateData, TEventDataType>(
            this IClaptrapGrain<TStateData> claptrapGrain,
            TEventDataType eventData,
            string uid)
            where TStateData : IStateData
            where TEventDataType : IEventData
        {
            // TODO event Type
            var dataEvent = new DataEvent(claptrapGrain.Actor.State.Identity,
                typeof(TEventDataType).FullName,
                eventData,
                new EventUid(uid));
            return dataEvent;
        }
    }
}