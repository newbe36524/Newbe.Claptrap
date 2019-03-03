using System.Collections.Generic;
using System.Linq;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventChannels;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Autofac
{
    public interface IEventPublishChannelFactory
    {
        IEnumerable<IEventPublishChannel> Create(IActorIdentity identity);
    }

    public class EventPublishChannelFactory : IEventPublishChannelFactory
    {
        private readonly IActorMetadataProvider _actorMetadataProvider;
        private readonly IEventPublishChannelProvider _eventPublishChannelProvider;

        public EventPublishChannelFactory(
            IActorMetadataProvider actorMetadataProvider,
            IEventPublishChannelProvider eventPublishChannelProvider)
        {
            _actorMetadataProvider = actorMetadataProvider;
            _eventPublishChannelProvider = eventPublishChannelProvider;
        }

        public IEnumerable<IEventPublishChannel> Create(IActorIdentity identity)
        {
            var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();
            var claptrapMetadata = actorMetadataCollection[(IClaptrapKind) identity.Kind];
            var channels =
                claptrapMetadata.MinionMetadata.Select(x =>
                    _eventPublishChannelProvider.Create(identity, x.MinionKind));
            return channels;
        }
    }
}