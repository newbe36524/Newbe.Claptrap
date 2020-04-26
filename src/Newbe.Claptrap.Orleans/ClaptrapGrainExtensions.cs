using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Orleans
{
    public static class ClaptrapGrainExtensions
    {
        public static DataEvent CreateEvent<TStateData, TEventDataType>(
            this IClaptrap<TStateData> claptrap,
            TEventDataType eventData)
            where TStateData : IStateData
            where TEventDataType : IEventData
        {
            return CreateEvent(claptrap, eventData, Guid.NewGuid().ToString());
        }

        public static DataEvent CreateEvent<TStateData, TEventDataType>(
            this IClaptrap<TStateData> claptrap,
            TEventDataType eventData,
            string uid)
            where TStateData : IStateData
            where TEventDataType : IEventData
        {
            // TODO event Type
            var dataEvent = new DataEvent(claptrap.Actor.State.Identity,
                typeof(TEventDataType).FullName,
                eventData,
                uid);
            return dataEvent;
        }
    }
}