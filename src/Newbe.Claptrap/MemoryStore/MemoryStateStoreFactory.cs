using System.Diagnostics.CodeAnalysis;

namespace Newbe.Claptrap.MemoryStore
{
    [ExcludeFromCodeCoverage]
    public class MemoryStateStoreFactory :
        IClaptrapComponentFactory<IStateSaver>,
        IClaptrapComponentFactory<IStateLoader>
    {
        private readonly MemoryStateStore.Factory _factory;

        public MemoryStateStoreFactory(
            MemoryStateStore.Factory factory)
        {
            _factory = factory;
        }

        IStateLoader IClaptrapComponentFactory<IStateLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory(claptrapIdentity);
        }

        IStateSaver IClaptrapComponentFactory<IStateSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory(claptrapIdentity);
        }
    }
}