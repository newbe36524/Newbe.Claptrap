using System.Collections.Generic;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.EventStore.Memory;

namespace Newbe.Claptrap.Autofac
{
    public class EventStoreFactory : IEventStoreFactory
    {
        private readonly Dictionary<string, IEventStore> _dictionary = new Dictionary<string, IEventStore>();

        public IEventStore Create(IActorIdentity identity)
        {
            // todo this is not impl
            if (!_dictionary.TryGetValue(identity.Id, out var store))
            {
                store = new MemoryEventStore(identity);
                _dictionary.Add(identity.Id, store);
            }

            return store;
        }
    }
}