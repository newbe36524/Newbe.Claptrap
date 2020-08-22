using System;
using System.Security.Cryptography;
using System.Text;
using Autofac;
using Newbe.Claptrap.Saga;

// ReSharper disable MemberCanBePrivate.Global

namespace Newbe.Claptrap.Orleans
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
            this IClaptrapBoxGrain<TStateData> claptrapBoxGrain,
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
            this IClaptrapBoxGrain<TStateData> claptrapBoxGrain,
            string flowKey,
            Type? userDataType = null)
            where TStateData : IStateData
            =>
                claptrapBoxGrain.CreateSagaClaptrap(() =>
                {
                    var state = claptrapBoxGrain.Claptrap.State;
                    return new SagaClaptrapIdentity(state.Identity, flowKey, userDataType ?? typeof(object));
                });

        /// <summary>
        /// Create a SagaClaptrap to handle saga flow
        /// </summary>
        /// <param name="claptrapBoxGrain"></param>
        /// <param name="sagaClaptrapIdFactory"></param>
        /// <typeparam name="TStateData"></typeparam>
        /// <returns></returns>
        public static IDisposableSagaClaptrap CreateSagaClaptrap<TStateData>(
            this IClaptrapBoxGrain<TStateData> claptrapBoxGrain,
            Func<ISagaClaptrapIdentity> sagaClaptrapIdFactory) where TStateData : IStateData
        {
            var commonService = claptrapBoxGrain.ClaptrapGrainCommonService;
            var scope = commonService.LifetimeScope.BeginLifetimeScope();
            var re = scope.CreateSagaClaptrap(sagaClaptrapIdFactory);
            return re;
        }

        /// <summary>
        /// Create a SagaClaptrap to handle saga flow
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="masterIdentity"></param>
        /// <param name="flowKey"></param>
        /// <param name="userDataType"></param>
        /// <returns></returns>
        public static IDisposableSagaClaptrap CreateSagaClaptrap(this ILifetimeScope scope,
            IClaptrapIdentity masterIdentity,
            string flowKey,
            Type? userDataType = null)
            =>
                scope.CreateSagaClaptrap(() =>
                    new SagaClaptrapIdentity(masterIdentity, flowKey, userDataType ?? typeof(object)));

        /// <summary>
        /// Create a SagaClaptrap to handle saga flow
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="sagaClaptrapIdFactory"></param>
        /// <returns></returns>
        public static IDisposableSagaClaptrap CreateSagaClaptrap(
            this ILifetimeScope scope,
            Func<ISagaClaptrapIdentity> sagaClaptrapIdFactory)
        {
            var factory = scope.Resolve<SagaClaptrap.Factory>();
            var sagaClaptrapIdentity = sagaClaptrapIdFactory();
            var sagaClaptrap = factory.Invoke(sagaClaptrapIdentity);
            var re = new DisposableSagaClaptrap(sagaClaptrap, scope);
            return re;
        }
    }
}