using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap.Autofac
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