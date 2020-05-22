using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.StateStore
{
    public class RelationDatabaseStateSaver<T> : IStateSaver
        where T : IStateEntity
    {
        private readonly IStateEntityMapper<T> _mapper;
        private readonly IStateEntitySaver<T> _stateEntitySaver;

        public RelationDatabaseStateSaver(IClaptrapIdentity identity,
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