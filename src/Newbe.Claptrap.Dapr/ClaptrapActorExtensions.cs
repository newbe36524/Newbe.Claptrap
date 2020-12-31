using System;

// ReSharper disable MemberCanBePrivate.Global
namespace Newbe.Claptrap.Dapr
{
    public static class ClaptrapActorExtensions
    {
        /// <summary>
        /// Create a event for claptrap to handle
        /// </summary>
        /// <param name="claptrapBoxActor"></param>
        /// <param name="eventData"></param>
        /// <typeparam name="TStateData"></typeparam>
        /// <typeparam name="TEventDataType"></typeparam>
        /// <returns></returns>
        public static DataEvent CreateEvent<TStateData, TEventDataType>(
            this IClaptrapBoxActor<TStateData> claptrapBoxActor,
            TEventDataType eventData)
            where TStateData : IStateData
            where TEventDataType : IEventData
        {
            var eventTypeCode =
                claptrapBoxActor.ClaptrapActorCommonService.ClaptrapTypeCodeFactory
                    .FindEventTypeCode(claptrapBoxActor, eventData);
            var dataEvent = new DataEvent(claptrapBoxActor.Claptrap.State.Identity,
                eventTypeCode,
                eventData);
            return dataEvent;
        }
    }
}