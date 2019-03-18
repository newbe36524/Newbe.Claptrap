using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventChannels;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.EventHub.DirectClient
{
    public class DirectClientEventPublishChannel : IEventPublishChannel
    {
        private readonly Func<IGrainFactory, IMinionGrain> _grainFunc;
        private readonly IGrainFactory _clusterClient;
        private readonly IDirectClient _directClient;

        public DirectClientEventPublishChannel(
            Func<IGrainFactory, IMinionGrain> grainFunc,
            IGrainFactory clusterClient,
            Type minionInterfaceType,
            IDirectClient directClient)
        {
            _grainFunc = grainFunc;
            _clusterClient = clusterClient;
            _directClient = directClient;
            _methodInfos = GetMethodInfos(minionInterfaceType);
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        private readonly IReadOnlyDictionary<string, MethodInfo> _methodInfos;


        private static IReadOnlyDictionary<string, MethodInfo> GetMethodInfos(Type interfaceType)
        {
            var re = new Dictionary<string, MethodInfo>();
            foreach (var methodInfo in interfaceType.GetMethods())
            {
                var minionEventAttribute = methodInfo.GetCustomAttribute<MinionEventAttribute>();
                if (!string.IsNullOrEmpty(minionEventAttribute?.EventType))
                {
                    re[minionEventAttribute.EventType] = methodInfo;
                }
            }

            return re;
        }

        public Task Publish(IEvent @event)
        {
            var minionGrain = _grainFunc(_clusterClient);
            _methodInfos.TryGetValue(@event.EventType, out var methodInfo);
            return _directClient.PublishEvent(minionGrain, @event, methodInfo);
        }
    }
}