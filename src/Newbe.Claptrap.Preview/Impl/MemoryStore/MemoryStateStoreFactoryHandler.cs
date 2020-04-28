using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.StateStore;

namespace Newbe.Claptrap.Preview
{
    [ExcludeFromCodeCoverage]
    public class MemoryStateStoreFactoryHandler : IStateStoreFactoryHandler
    {
        private readonly MemoryStateStore.Factory _memoryStateStoreFactory;

        public MemoryStateStoreFactoryHandler(
            MemoryStateStore.Factory memoryStateStoreFactory)
        {
            _memoryStateStoreFactory = memoryStateStoreFactory;
        }

        public IStateStore Create(IActorIdentity identity)
        {
            return _memoryStateStoreFactory(identity);
        }
    }
}