using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventChannels;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.EventHub.DirectClient
{
    public class DirectClientEventPublishChannelProvider : IEventPublishChannelProvider
    {
        private readonly IGrainFactory _clusterClient;
        private readonly IActorMetadataProvider _actorMetadataProvider;

        public DirectClientEventPublishChannelProvider(
            IGrainFactory clusterClient,
            IActorMetadataProvider actorMetadataProvider)
        {
            _clusterClient = clusterClient;
            _actorMetadataProvider = actorMetadataProvider;
        }

        public IEventPublishChannel Create(IActorIdentity claptrapIdentity, IMinionKind minionKind)
        {
            var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();
            var minionMetadata = actorMetadataCollection[minionKind];
            var directClientEventPublishChannel = new DirectClientEventPublishChannel(client =>
            {
                var methodInfo = typeof(IGrainFactory).GetMethod(nameof(IGrainFactory.GetGrain),
                    new[] {typeof(string), typeof(string)});
                var method = methodInfo.MakeGenericMethod(minionMetadata.InterfaceType);
                var grain =
                    (IMinionGrain) method.Invoke(client, new object[] {claptrapIdentity.Id, string.Empty});

                return grain;
            }, _clusterClient, minionMetadata.InterfaceType);
            return directClientEventPublishChannel;
        }
    }
}