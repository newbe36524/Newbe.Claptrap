using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        private readonly Type _minionInterfaceType;
        private readonly BufferBlock<IEvent> _bufferBlock;

        public DirectClientEventPublishChannel(
            Func<IGrainFactory, IMinionGrain> grainFunc,
            IGrainFactory clusterClient,
            Type minionInterfaceType)
        {
            _grainFunc = grainFunc;
            _clusterClient = clusterClient;
            _minionInterfaceType = minionInterfaceType;
            _bufferBlock = new BufferBlock<IEvent>();
            // todo need to keep order 
            _bufferBlock.LinkTo(new ActionBlock<IEvent>(PublishEvent));
        }

        private readonly ConcurrentDictionary<string, MethodInfo>
            _methodInfos = new ConcurrentDictionary<string, MethodInfo>();

        private async Task PublishEvent(IEvent @event)
        {
            while (true)
            {
                try
                {
                    var minion = _grainFunc(_clusterClient);
                    var methodInfo = _methodInfos.GetOrAdd(@event.EventType, GetMethodInfo);
                    if (methodInfo != null)
                    {
                        var task = (Task) methodInfo.Invoke(minion, new object[] {@event});
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

        private MethodInfo GetMethodInfo(string eventType)
        {
            foreach (var methodInfo in _minionInterfaceType.GetMethods())
            {
                var minionEventAttribute = methodInfo.GetCustomAttribute<MinionEventAttribute>();
                if (minionEventAttribute?.EventType == eventType)
                {
                    return methodInfo;
                }
            }

            return null;
        }

        public Task Publish(IEvent @event)
        {
            return _bufferBlock.SendAsync(@event);
        }
    }
}