using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Preview.Abstractions;
using Newbe.Claptrap.Preview.Impl.MemoryStore;

namespace Newbe.Claptrap.Preview.Impl.Modules
{
    [ExcludeFromCodeCoverage]
    public class MemoryStorageModule : StorageSupportModule
    {
        public MemoryStorageModule()
        {
            EventStoreType = typeof(MemoryEventStore);

            StateStoreType = typeof(MemoryStateStore);
        }
    }
}