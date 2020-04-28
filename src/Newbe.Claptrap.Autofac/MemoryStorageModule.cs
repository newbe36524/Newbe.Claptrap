using Autofac;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap.Autofac
{
    public class MemoryStorageModule : StorageSupportModule
    {
        public MemoryStorageModule() : base(EventStoreProvider.Memory)
        {
            EventStoreType = typeof(MemoryEventStore);
            EventStoreFactoryHandlerType = typeof(MemoryEventStoreFactoryHandler);
        }
    }
}