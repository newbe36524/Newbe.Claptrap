using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventStore;

namespace Newbe.Claptrap.Preview
{
    [ExcludeFromCodeCoverage]
    public class MemoryEventStoreFactoryHandler : IIEventStoreFactoryHandler
    {
        private readonly MemoryEventStore.Factory _memoryEventStoreFactory;

        public MemoryEventStoreFactoryHandler(
            MemoryEventStore.Factory memoryEventStoreFactory)
        {
            _memoryEventStoreFactory = memoryEventStoreFactory;
        }

        public IEventStore Create(IActorIdentity identity)
        {
            return _memoryEventStoreFactory(identity);
        }
    }
}