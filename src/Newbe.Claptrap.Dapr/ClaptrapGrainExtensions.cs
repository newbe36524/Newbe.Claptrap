using System;
using Newbe.Claptrap.Saga;

// ReSharper disable MemberCanBePrivate.Global
namespace Newbe.Claptrap.Dapr
{
    public static class ClaptrapGrainExtensions
    {
        /// <summary>
        /// Create a event for claptrap to handle
        /// </summary>
        /// <param name="claptrapBoxGrain"></param>
        /// <param name="eventData"></param>
        /// <typeparam name="TStateData"></typeparam>
        /// <typeparam name="TEventDataType"></typeparam>
        /// <returns></returns>
        public static DataEvent CreateEvent<TStateData, TEventDataType>(
            this IClaptrapBoxActor<TStateData> claptrapBoxGrain,
            TEventDataType eventData)
            where TStateData : IStateData
            where TEventDataType : IEventData
        {
            var eventTypeCode =
                claptrapBoxGrain.ClaptrapGrainCommonService.ClaptrapTypeCodeFactory
                    .FindEventTypeCode(claptrapBoxGrain, eventData);
            var dataEvent = new DataEvent(claptrapBoxGrain.Claptrap.State.Identity,
                eventTypeCode,
                eventData);
            return dataEvent;
        }

        /// <summary>
        /// Create a SagaClaptrap to handle saga flow
        /// </summary>
        /// <param name="claptrapBoxGrain"></param>
        /// <param name="flowKey">Key of flow. It must be different if you want to create multiple saga flow in a ClaptrapGrainBox</param>
        /// <param name="userDataType"></param>
        /// <typeparam name="TStateData"></typeparam>
        /// <returns></returns>
        public static IDisposableSagaClaptrap CreateSagaClaptrap<TStateData>(
            this IClaptrapBoxActor<TStateData> claptrapBoxGrain,
            string flowKey,
            Type? userDataType = null)
            where TStateData : IStateData
        {
            return claptrapBoxGrain.CreateSagaClaptrap(() =>
            {
                var state = claptrapBoxGrain.Claptrap.State;
                return new SagaClaptrapIdentity(state.Identity, flowKey, userDataType ?? typeof(object));
            });
        }

        /// <summary>
        /// Create a SagaClaptrap to handle saga flow
        /// </summary>
        /// <param name="claptrapBoxGrain"></param>
        /// <param name="sagaClaptrapIdFactory"></param>
        /// <typeparam name="TStateData"></typeparam>
        /// <returns></returns>
        public static IDisposableSagaClaptrap CreateSagaClaptrap<TStateData>(
            this IClaptrapBoxActor<TStateData> claptrapBoxGrain,
            Func<ISagaClaptrapIdentity> sagaClaptrapIdFactory) where TStateData : IStateData
        {
            var commonService = claptrapBoxGrain.ClaptrapGrainCommonService;
            var scope = commonService.LifetimeScope.BeginLifetimeScope();
            var re = scope.CreateSagaClaptrap(sagaClaptrapIdFactory);
            return re;
        }
    }
}