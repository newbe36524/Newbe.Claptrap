using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable
{
    public class SharedTableEventSaverProvider : IEventSaverProvider
    {
        private readonly ILogger<SharedTableEventSaverProvider> _logger;
        private readonly IClock _clock;
        private readonly ISharedTableEventStoreProvider _sharedTableEventStoreProvider;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public SharedTableEventSaverProvider(
            ILogger<SharedTableEventSaverProvider> logger,
            IClock clock,
            ISharedTableEventStoreProvider sharedTableEventStoreProvider,
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _sharedTableEventStoreProvider = sharedTableEventStoreProvider;
            _eventDataStringSerializer = eventDataStringSerializer;
            _logger = logger;
            _clock = clock;
        }

        public async Task SaveOneAsync(IEvent @event)
        {
            var eventData =
                _eventDataStringSerializer.Serialize(@event.ClaptrapIdentity.TypeCode, @event.EventTypeCode,
                    @event.Data);
            var eventEntity = new SharedTableEventEntity
            {
                Version = @event.Version,
                CreatedTime = _clock.UtcNow,
                EventData = eventData,
                EventTypeCode = @event.EventTypeCode,
                ClaptrapId = @event.ClaptrapIdentity.Id,
                ClaptrapTypeCode = @event.ClaptrapIdentity.TypeCode,
            };
            _logger.LogDebug("start to save event to store {@eventEntity}", eventEntity);

            await _sharedTableEventStoreProvider.InsertOneAsync(eventEntity);
        }

        public async Task SaveManyAsync(IEnumerable<IEvent> events)
        {
            var entities = events.Select(@event =>
            {
                var eventData =
                    _eventDataStringSerializer.Serialize(@event.ClaptrapIdentity.TypeCode, @event.EventTypeCode,
                        @event.Data);
                var eventEntity = new SharedTableEventEntity
                {
                    Version = @event.Version,
                    CreatedTime = _clock.UtcNow,
                    EventData = eventData,
                    EventTypeCode = @event.EventTypeCode,
                    ClaptrapId = @event.ClaptrapIdentity.Id,
                    ClaptrapTypeCode = @event.ClaptrapIdentity.TypeCode,
                };
                return eventEntity;
            });

            await _sharedTableEventStoreProvider.InsertManyAsync(entities);
        }
    }
}