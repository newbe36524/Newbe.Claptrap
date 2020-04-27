using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap.Autofac
{
    [ExcludeFromCodeCoverage]
    public class MemoryStateStore : IStateStore
    {
        public delegate MemoryStateStore Factory(IActorIdentity identity);

        private readonly IInitialStateDataFactory _initialStateDataFactory;

        public MemoryStateStore(
            IActorIdentity identity,
            IInitialStateDataFactory initialStateDataFactory)
        {
            _initialStateDataFactory = initialStateDataFactory;
            Identity = identity;
        }

        public IActorIdentity Identity { get; }
        private IState? _state;

        public async Task<IState?> GetStateSnapshot()
        {
            var stateData = await _initialStateDataFactory.Create(Identity);
            _state = new DataState(Identity, stateData, 0);
            return _state;
        }

        public Task Save(IState state)
        {
            _state = state;
            return Task.CompletedTask;
        }
    }
}