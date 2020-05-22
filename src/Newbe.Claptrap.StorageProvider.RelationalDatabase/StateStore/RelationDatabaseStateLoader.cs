using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.StateStore
{
    public class RelationDatabaseStateLoader<T> : IStateLoader
        where T : IStateEntity
    {
        private readonly IStateEntityMapper<T> _mapper;
        private readonly IStateEntityLoader<T> _stateEntityLoader;

        public RelationDatabaseStateLoader(IClaptrapIdentity identity,
            IStateEntityMapper<T> mapper,
            IStateEntityLoader<T> stateEntityLoader)
        {
            Identity = identity;
            _mapper = mapper;
            _stateEntityLoader = stateEntityLoader;
        }

        public IClaptrapIdentity Identity { get; }

        public async Task<IState?> GetStateSnapshotAsync()
        {
            var entity = await _stateEntityLoader.GetStateSnapshotAsync();
            if (entity == null)
            {
                return null;
            }

            var re = _mapper.Map(entity, Identity);
            return re;
        }
    }
}