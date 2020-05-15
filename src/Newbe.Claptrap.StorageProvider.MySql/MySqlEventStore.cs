using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlEventStore : IEventLoader, IEventSaver
    {
        public delegate MySqlEventStore Factory(IClaptrapIdentity identity);

        public MySqlEventStore(IClaptrapIdentity identity)
        {
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task SaveEventAsync(IEvent @event)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            throw new System.NotImplementedException();
        }
    }
}