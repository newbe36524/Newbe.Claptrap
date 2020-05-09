using System.Diagnostics.CodeAnalysis;

namespace Newbe.Claptrap.MemoryStore
{
    [ExcludeFromCodeCoverage]
    public class MemoryEventStoreFactory :
        IClaptrapComponentFactory<IEventLoader>,
        IClaptrapComponentFactory<IEventSaver>
    {
        private readonly MemoryEventStore.Factory _memoryEventStoreFactory;

        public MemoryEventStoreFactory(
            MemoryEventStore.Factory memoryEventStoreFactory)
        {
            _memoryEventStoreFactory = memoryEventStoreFactory;
        }

        IEventLoader IClaptrapComponentFactory<IEventLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _memoryEventStoreFactory(claptrapIdentity);
        }

        IEventSaver IClaptrapComponentFactory<IEventSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _memoryEventStoreFactory(claptrapIdentity);
        }
    }
}