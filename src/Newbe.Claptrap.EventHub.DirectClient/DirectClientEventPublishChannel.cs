using System.Collections.Generic;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventChannels;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.EventHub.DirectClient
{
    public class DirectClientEventPublishChannel : IEventPublishChannel
    {
        private readonly IMinionGrain _minionGrain;
        private readonly Dictionary<string, MinionEventMethodMetadata> _minionEventMethodMetadataDic;
        private readonly IDirectClient _directClient;

        public DirectClientEventPublishChannel(
            IMinionGrain minionGrain,
            Dictionary<string, MinionEventMethodMetadata> minionEventMethodMetadataDic,
            IDirectClient directClient)
        {
            _minionGrain = minionGrain;
            _minionEventMethodMetadataDic = minionEventMethodMetadataDic;
            _directClient = directClient;
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public Task Publish(IEvent @event)
        {
            _minionEventMethodMetadataDic.TryGetValue(@event.EventType, out var metadata);
            return _directClient.PublishEvent(_minionGrain, @event, metadata?.MethodInfo);
        }
    }
}