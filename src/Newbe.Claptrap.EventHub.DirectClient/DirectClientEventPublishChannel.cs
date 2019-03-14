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

        public DirectClientEventPublishChannel(
            Func<IGrainFactory, IMinionGrain> grainFunc,
            IGrainFactory clusterClient,
            Type minionInterfaceType)
        {
            _grainFunc = grainFunc;
            _clusterClient = clusterClient;
            _methodInfos = GetMethodInfos(minionInterfaceType);
            Task.Factory.StartNew(PublishEvent);
        }

        private readonly IReadOnlyDictionary<string, MethodInfo> _methodInfos;

        private readonly ConcurrentQueue<IEvent> _queue =
            new ConcurrentQueue<IEvent>();

        private async Task PublishEvent()
        {
            while (true)
            {
                while (_queue.TryDequeue(out var @event))
                {
                    await PublishEvent(@event);
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async Task PublishEvent(IEvent @event)
        {
            while (true)
            {
                try
                {
                    if (_methodInfos.TryGetValue(@event.EventType, out var methodInfo))
                    {
                        var minionGrain = _grainFunc(_clusterClient);
                        var task = (Task) methodInfo.Invoke(minionGrain, new object[] {@event});
                        await task;
                    }

                    return;
                }
                catch (Exception e)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    // todo log error message
                    Console.WriteLine(e);
                }
            }
        }

        private IReadOnlyDictionary<string, MethodInfo> GetMethodInfos(Type interfaceType)
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
            _queue.Enqueue(@event);
            return Task.CompletedTask;
        }
    }
}