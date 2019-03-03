using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;
using Orleans;

namespace Newbe.Claptrap.EventHub.Memory
{
    public class EventHubManager : IEventHubManager
    {
        private readonly IActorMetadataProvider _actorMetadataProvider;
        private readonly IClusterClient _clusterClient;

        public EventHubManager(
            IActorMetadataProvider actorMetadataProvider,
            IClusterClient clusterClient)
        {
            _actorMetadataProvider = actorMetadataProvider;
            _clusterClient = clusterClient;
        }

        private readonly ConcurrentDictionary<IActorKind, BufferBlock<IEvent>> _dictionary
            = new ConcurrentDictionary<IActorKind, BufferBlock<IEvent>>();

        public Task Publish(IActorIdentity @from, IEvent @event)
        {
            if (!_dictionary.TryGetValue(@from.Kind, out var block))
            {
                var newBlock = CreateBlock(from.Kind);
                _dictionary.AddOrUpdate(from.Kind, newBlock, (kind, bufferBlock) => CreateBlock(kind));
                block = newBlock;
            }

            return block.SendAsync(@event);
        }

        private BufferBlock<IEvent> CreateBlock(IActorKind kind)
        {
            var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();
            var claptrapMetadata = actorMetadataCollection[(IClaptrapKind) kind];
            var eventReceiveChannels = claptrapMetadata.MinionMetadata
                .Select(x => new EventReceiveChannel(_clusterClient, x.MinionKind))
                .ToArray();
            var bufferBlock = new BufferBlock<IEvent>();
            var broadcastBlock = new BroadcastBlock<IEvent>(x => x);
            bufferBlock.LinkTo(broadcastBlock);
            foreach (var eventReceiveChannel in eventReceiveChannels)
            {
                broadcastBlock.LinkTo(
                    new ActionBlock<IEvent>(new Func<IEvent, Task>(@event => eventReceiveChannel.Receive(@event))));
            }

            return bufferBlock;
        }
    }
}