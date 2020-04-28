using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap.Autofac
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