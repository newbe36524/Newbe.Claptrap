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
        private bool _disposed;

        public DirectClientEventPublishChannel(
            Func<IGrainFactory, IMinionGrain> grainFunc,
            IGrainFactory clusterClient,
            Type minionInterfaceType)
        {
            _grainFunc = grainFunc;
            _clusterClient = clusterClient;
            _methodInfos = GetMethodInfos(minionInterfaceType);
            Task.Run(PublishEvent);
        }

        public async ValueTask DisposeAsync()
        {
            _disposed = true;
            while (_queue.TryDequeue(out var @event))
            {
                await PublishEvent(@event);
            }
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
                    if (_disposed)
                    {
                        return;
                    }

                    await PublishEvent(@event);
                }

                if (_disposed)
                {
                    return;
                }

                await Task.Delay(1000);
            }
        }

        private async Task PublishEvent(IEvent @event)
        {
            while (true)
            {
                try
                {
                    var minionGrain = _grainFunc(_clusterClient);
                    Task task;
                    if (_methodInfos.TryGetValue(@event.EventType, out var methodInfo))
                    {
                        task = (Task) methodInfo.Invoke(minionGrain, new object[] {@event});
                    }
                    else
                    {
                        task = minionGrain.HandleOtherEvent(@event);
                    }

                    await task;
                }
                catch (Exception e)
                {
                    await Task.Delay(1000);
                    // todo log error message
                    Console.WriteLine(e);
                }
            }
        }

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
            _queue.Enqueue(@event);
            return Task.CompletedTask;
        }
    }
}