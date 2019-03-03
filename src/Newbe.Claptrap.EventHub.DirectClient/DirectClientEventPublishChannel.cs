using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventChannels;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.EventHub.DirectClient
{
    public class DirectClientEventPublishChannel : IEventPublishChannel
    {
        private readonly Func<IGrainFactory, IMinionGrain> _grainFunc;
        private readonly IGrainFactory _clusterClient;
        private readonly BufferBlock<IEvent> _bufferBlock;

        public DirectClientEventPublishChannel(
            Func<IGrainFactory, IMinionGrain> grainFunc,
            IGrainFactory clusterClient)
        {
            _grainFunc = grainFunc;
            _clusterClient = clusterClient;
            _bufferBlock = new BufferBlock<IEvent>();
            _bufferBlock.LinkTo(new ActionBlock<IEvent>(PublishEvent));
        }

        private async Task PublishEvent(IEvent @event)
        {
            while (true)
            {
                try
                {
                    var minion = _grainFunc(_clusterClient);
                    await minion.HandleEvent(@event);
                    break;
                }
                catch (Exception e)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    // todo log error message
                    Console.WriteLine(e);
                }
            }
        }

        public Task Publish(IEvent @event)
        {
            return _bufferBlock.SendAsync(@event);
        }
    }
}