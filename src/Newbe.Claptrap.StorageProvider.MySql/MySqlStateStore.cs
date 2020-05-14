using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlStateStore : IStateLoader, IStateSaver
    {
        public delegate MySqlStateStore Factory(IClaptrapIdentity identity);

        public MySqlStateStore(IClaptrapIdentity identity)
        {
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task SaveAsync(IState state)
        {
            throw new System.NotImplementedException();
        }

        public Task<IState?> GetStateSnapshotAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}