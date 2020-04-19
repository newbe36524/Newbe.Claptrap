using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap.Autofac
{
    public class StateStoreFactory : IStateStoreFactory
    {
        private readonly MemoryStateStore.Factory _factory;

        public StateStoreFactory(
            MemoryStateStore.Factory factory)
        {
            _factory = factory;
        }

        public IStateStore Create(IActorIdentity identity)
        {
            // TODO impl
            return _factory(identity);
        }
    }
}