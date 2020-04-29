using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl.MemoryStore
{
    [ExcludeFromCodeCoverage]
    public class MemoryStateStore : IStateLoader, IStateSaver
    {
        public delegate MemoryStateStore Factory(IClaptrapIdentity identity);

        private readonly IInitialStateDataFactory _initialStateDataFactory;

        public MemoryStateStore(
            IClaptrapIdentity identity,
            IInitialStateDataFactory initialStateDataFactory)
        {
            _initialStateDataFactory = initialStateDataFactory;
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }
        private IState? _state;

        public async Task<IState?> GetStateSnapshotAsync()
        {
            var stateData = await _initialStateDataFactory.Create(Identity);
            _state = new DataState(Identity, stateData, 0);
            return _state;
        }

        public Task SaveAsync(IState state)
        {
            _state = state;
            return Task.CompletedTask;
        }
    }
}