using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StateStore.Memory
{
    public class MemoryStateStore : IStateStore
    {
        private IState _state = null;

        public MemoryStateStore(IActorIdentity identity)
        {
            Identity = identity;
        }


        public IActorIdentity Identity { get; }

        public Task<IState> GetStateSnapshot()
        {
            return Task.FromResult(_state);
        }

        public Task Save(IState state)
        {
            _state = state;
            return Task.CompletedTask;
        }
    }
}