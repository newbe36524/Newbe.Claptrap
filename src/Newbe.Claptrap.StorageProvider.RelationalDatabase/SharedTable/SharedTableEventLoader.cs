using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable
{
    public class SharedTableEventLoader : IEventLoader
    {
        private readonly ILogger<SharedTableEventLoader> _logger;
        private readonly ISharedTableEventStoreProvider _sharedTableEventStoreProvider;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public SharedTableEventLoader(IClaptrapIdentity identity,
            ILogger<SharedTableEventLoader> logger,
            ISharedTableEventStoreProvider sharedTableEventStoreProvider,
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _logger = logger;
            _sharedTableEventStoreProvider = sharedTableEventStoreProvider;
            _eventDataStringSerializer = eventDataStringSerializer;
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public async Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            var ps = new {startVersion, endVersion};
            var eventEntities = await _sharedTableEventStoreProvider.SelectAsync(startVersion, endVersion);

            var re = eventEntities.Select(x =>
            {
                var eventData = _eventDataStringSerializer.Deserialize(Identity.TypeCode, x.EventTypeCode, x.EventData);
                var dataEvent = new DataEvent(Identity, x.EventTypeCode, eventData)
                {
                    Version = x.Version
                };
                return dataEvent;
            }).ToArray();
            _logger.LogDebug("found {count} events that version in range [{startVersion}, {endVersion}).",
                re.Length,
                startVersion,
                endVersion);
            return re;
        }
    }
}