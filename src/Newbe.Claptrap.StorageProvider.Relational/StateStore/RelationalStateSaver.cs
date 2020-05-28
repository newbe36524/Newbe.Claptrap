using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class RelationalStateSaver<T> : IRelationalStateSaver
        where T : IStateEntity
    {
        private readonly IStateEntityMapper<T> _mapper;
        private readonly IStateEntitySaver<T> _stateEntitySaver;

        public RelationalStateSaver(IClaptrapIdentity identity,
            IStateEntityMapper<T> mapper,
            IStateEntitySaver<T> stateEntitySaver)
        {
            Identity = identity;
            _mapper = mapper;
            _stateEntitySaver = stateEntitySaver;
        }

        public IClaptrapIdentity Identity { get; }

        public async Task SaveAsync(IState state)
        {
            var stateEntity = _mapper.Map(state);
            await _stateEntitySaver.SaveAsync(stateEntity);
        }
    }
}