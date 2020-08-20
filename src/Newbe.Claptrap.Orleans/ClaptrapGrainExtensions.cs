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
        /// <typeparam name="TStateData"></typeparam>
        /// <returns></returns>
        public static IDisposableSagaClaptrap CreateSagaClaptrap<TStateData>(
            this IClaptrapBoxGrain<TStateData> claptrapBoxGrain, string flowKey)
            where TStateData : IStateData
            =>
                claptrapBoxGrain.CreateSagaClaptrap(() =>
                {
                    var state = claptrapBoxGrain.Claptrap.State;
                    var hashAlgorithm = HashAlgorithm.Create();
                    var sourceId = $"{flowKey}_{state.Identity.Id}_{state.Identity.TypeCode}";
                    var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(sourceId));
                    var hashKey = new StringBuilder();
                    foreach (var b in hash)
                    {
                        hashKey.Append(b.ToString("x2"));
                    }

                    return hashKey.ToString();
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
            Func<string> sagaClaptrapIdFactory) where TStateData : IStateData
        {
            var commonService = claptrapBoxGrain.ClaptrapGrainCommonService;
            var scope = commonService.LifetimeScope.BeginLifetimeScope();
            var factory = scope.Resolve<SagaClaptrap.Factory>();
            var id = sagaClaptrapIdFactory();
            var sagaIdentity = new ClaptrapIdentity(id, SagaCodes.ClaptrapTypeCode);
            var sagaClaptrap = factory.Invoke(sagaIdentity);
            var re = new DisposableSagaClaptrap(sagaClaptrap, scope);
            return re;
        }
    }
}