using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class RelationalStateLoader<T> : IRelationalStateLoader
        where T : class, IStateEntity
    {
        private readonly IStateEntityMapper<T> _mapper;
        private readonly IStateEntityLoader<T> _stateEntityLoader;

        public RelationalStateLoader(IClaptrapIdentity identity,
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
            var entity = await _stateEntityLoader.GetStateSnapshotAsync().ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            var re = _mapper.Map(entity, Identity);
            return re;
        }
    }
}