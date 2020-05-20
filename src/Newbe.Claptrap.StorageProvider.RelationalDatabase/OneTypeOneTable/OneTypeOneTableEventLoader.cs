using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.OneTypeOneTable
{
    public class OneTypeOneTableEventLoader : IEventLoader
    {
        private readonly ILogger<OneTypeOneTableEventLoader> _logger;
        private readonly IOneTypeOneTableEventStoreProvider _oneTypeOneTableEventStoreProvider;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public OneTypeOneTableEventLoader(
            IClaptrapIdentity identity,
            ILogger<OneTypeOneTableEventLoader> logger,
            IOneTypeOneTableEventStoreProvider oneTypeOneTableEventStoreProvider,
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _logger = logger;
            _oneTypeOneTableEventStoreProvider = oneTypeOneTableEventStoreProvider;
            _eventDataStringSerializer = eventDataStringSerializer;
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public async Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            _logger.LogDebug("start to get events that version in range [{startVersion}, {endVersion}).",
                startVersion,
                endVersion);
            var eventEntities = await _oneTypeOneTableEventStoreProvider.SelectAsync(startVersion, endVersion);

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