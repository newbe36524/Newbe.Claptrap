using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public abstract class DecoratedEventLoader : IEventLoader
    {
        public IEventLoader EventLoader { get; }
        public IClaptrapIdentity Identity => EventLoader.Identity;

        protected DecoratedEventLoader(
            IEventLoader loader)
        {
            EventLoader = loader;
        }

        public abstract Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion);
    }
}