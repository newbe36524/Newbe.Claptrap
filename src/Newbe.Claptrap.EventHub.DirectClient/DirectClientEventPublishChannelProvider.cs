using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        private readonly ConcurrentDictionary<IMinionKind, CacheItem>
            _minionEventMethodMetadataCache
                = new ConcurrentDictionary<IMinionKind, CacheItem>();

        private CacheItem GetMethodDic(IMinionKind minionKind)
        {
            if (_minionEventMethodMetadataCache.TryGetValue(minionKind, out var value))
            {
                return value;
            }

            var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();
            var minionMetadata = actorMetadataCollection[minionKind];
            var minionEventMethodMetadataDic = minionMetadata.MinionEventMethodMetadata
                .ToDictionary(x => x.ClaptrapEventMetadata.EventType);
            var cacheItem = new CacheItem
            {
                MinionEventMethodMetadata = minionEventMethodMetadataDic,
                InterfaceType = minionMetadata.InterfaceType
            };
            _minionEventMethodMetadataCache.TryAdd(minionKind, cacheItem);
            return cacheItem;
        }

        public IEventPublishChannel Create(IActorIdentity claptrapIdentity, IMinionKind minionKind)
        {
            var cacheItem = GetMethodDic(minionKind);
            var methodInfo = typeof(IGrainFactory).GetMethod(nameof(IGrainFactory.GetGrain),
                new[] {typeof(string), typeof(string)});
            var method = methodInfo.MakeGenericMethod(cacheItem.InterfaceType);
            var minionGrain =
                (IMinionGrain) method.Invoke(_clusterClient, new object[] {claptrapIdentity.Id, string.Empty});

            var directClientEventPublishChannel = new DirectClientEventPublishChannel(minionGrain,
                cacheItem.MinionEventMethodMetadata,
                DirectClient.Instance);
            return directClientEventPublishChannel;
        }

        private class CacheItem
        {
            public Dictionary<string, MinionEventMethodMetadata> MinionEventMethodMetadata { get; set; }
            public Type InterfaceType { get; set; }
        }
    }
}