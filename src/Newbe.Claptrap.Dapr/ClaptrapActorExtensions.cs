using System;
using Newbe.Claptrap.Saga;

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

        /// <summary>
        /// Create a SagaClaptrap to handle saga flow
        /// </summary>
        /// <param name="claptrapBoxActor"></param>
        /// <param name="flowKey">Key of flow. It must be different if you want to create multiple saga flow in a claptrapBoxActor</param>
        /// <param name="userDataType"></param>
        /// <typeparam name="TStateData"></typeparam>
        /// <returns></returns>
        public static IDisposableSagaClaptrap CreateSagaClaptrap<TStateData>(
            this IClaptrapBoxActor<TStateData> claptrapBoxActor,
            string flowKey,
            Type? userDataType = null)
            where TStateData : IStateData
        {
            return claptrapBoxActor.CreateSagaClaptrap(() =>
            {
                var state = claptrapBoxActor.Claptrap.State;
                return new SagaClaptrapIdentity(state.Identity, flowKey, userDataType ?? typeof(object));
            });
        }

        /// <summary>
        /// Create a SagaClaptrap to handle saga flow
        /// </summary>
        /// <param name="claptrapBoxActor"></param>
        /// <param name="sagaClaptrapIdFactory"></param>
        /// <typeparam name="TStateData"></typeparam>
        /// <returns></returns>
        public static IDisposableSagaClaptrap CreateSagaClaptrap<TStateData>(
            this IClaptrapBoxActor<TStateData> claptrapBoxActor,
            Func<ISagaClaptrapIdentity> sagaClaptrapIdFactory) where TStateData : IStateData
        {
            var commonService = claptrapBoxActor.ClaptrapActorCommonService;
            var scope = commonService.LifetimeScope.BeginLifetimeScope();
            var re = scope.CreateSagaClaptrap(sagaClaptrapIdFactory);
            return re;
        }
    }
}