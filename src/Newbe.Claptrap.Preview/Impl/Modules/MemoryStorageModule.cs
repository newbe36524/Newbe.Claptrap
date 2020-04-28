using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Preview.EventStore;

namespace Newbe.Claptrap.Preview
{
    [ExcludeFromCodeCoverage]
    public class MemoryStorageModule : StorageSupportModule
    {
        public MemoryStorageModule() : base(
            EventStoreProvider.Memory,
            StateStoreProvider.Memory)
        {
            EventStoreType = typeof(MemoryEventStore);
            EventStoreFactoryHandlerType = typeof(MemoryEventStoreFactoryHandler);

            StateStoreType = typeof(MemoryStateStore);
            StateStoreFactoryHandlerType = typeof(MemoryStateStoreFactoryHandler);
        }
    }
}