using System;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Impl;

namespace Newbe.Claptrap.Preview.Orleans
{
    public static class ClaptrapGrainExtensions
    {
        public static DataEvent CreateEvent<TStateData, TEventDataType>(
            this IClaptrapBox<TStateData> claptrapBox,
            TEventDataType eventData)
            where TStateData : IStateData
            where TEventDataType : IEventData
        {
            return CreateEvent(claptrapBox, eventData, Guid.NewGuid().ToString());
        }

        public static DataEvent CreateEvent<TStateData, TEventDataType>(
            this IClaptrapBox<TStateData> claptrapBox,
            TEventDataType eventData,
            string uid)
            where TStateData : IStateData
            where TEventDataType : IEventData
        {
            // TODO event Type
            var dataEvent = new DataEvent(claptrapBox.Claptrap.State.Identity,
                typeof(TEventDataType).FullName,
                eventData,
                uid);
            return dataEvent;
        }
    }
}