using System;
using System.Collections.Generic;
using System.Reflection;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.EventStore.Memory;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Autofac
{
    public class EventStoreFactory : IEventStoreFactory
    {
        private readonly IActorMetadataProvider _actorMetadataProvider;
        private readonly Dictionary<string, IEventStore> _dictionary = new Dictionary<string, IEventStore>();

        public EventStoreFactory(
            IActorMetadataProvider actorMetadataProvider)
        {
            _actorMetadataProvider = actorMetadataProvider;
        }

        public IEventStore Create(IActorIdentity identity)
        {
            // todo this is not impl
            if (!_dictionary.TryGetValue(identity.Id, out var store))
            {
                var actorMetadataCollection = _actorMetadataProvider.GetActorMetadata();
                switch (identity.Kind.ActorType)
                {
                    case ActorType.Claptrap:
                        var claptrapMetadata = actorMetadataCollection[(IClaptrapKind) identity.Kind];
                        if (claptrapMetadata.InterfaceType.GetCustomAttribute(typeof(TestEventStoreAttribute)) != null)
                        {
                            return new EmptyEventStore(identity);
                        }

                        break;
                    case ActorType.Minion:
                        var minionMetadata = actorMetadataCollection[(IMinionKind) identity.Kind];
                        if (minionMetadata.ClaptrapMetadata.InterfaceType.GetCustomAttribute(
                                typeof(TestEventStoreAttribute)) != null)
                        {
                            return new EmptyEventStore(identity);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                store = new MemoryEventStore(identity);
                _dictionary.Add(identity.Id, store);
            }

            return store;
        }
    }
}