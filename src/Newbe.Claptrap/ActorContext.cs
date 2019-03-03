using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap
{
    public class ActorContext : IActorContext
    {
        private readonly IStateInitializer _stateInitializer;

        public ActorContext(
            IActorIdentity identity,
            IStateInitializer stateInitializer)
        {
            _stateInitializer = stateInitializer;
            Identity = identity;
        }

        public IActorIdentity Identity { get; }
        public IState State { get; private set; }

        public async Task InitializeAsync()
        {
            var state = await _stateInitializer.InitializeAsync();
            State = state;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}