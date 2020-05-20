using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.OneTypeOneTable
{
    public class OneTypeOneTableEventSaverProvider : IEventSaverProvider
    {
        private readonly ILogger<OneTypeOneTableEventSaverProvider> _logger;
        private readonly IClock _clock;
        private readonly IOneTypeOneTableEventStoreProvider _oneTypeOneTableEventStoreProvider;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public OneTypeOneTableEventSaverProvider(
            ILogger<OneTypeOneTableEventSaverProvider> logger,
            IClock clock,
            IOneTypeOneTableEventStoreProvider oneTypeOneTableEventStoreProvider,
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _oneTypeOneTableEventStoreProvider = oneTypeOneTableEventStoreProvider;
            _eventDataStringSerializer = eventDataStringSerializer;
            _logger = logger;
            _clock = clock;
        }

        public async Task SaveOneAsync(IEvent @event)
        {
            var eventData =
                _eventDataStringSerializer.Serialize(@event.ClaptrapIdentity.TypeCode, @event.EventTypeCode,
                    @event.Data);
            var eventEntity = new OneTypeOneTableEventEntity
            {
                Version = @event.Version,
                CreatedTime = _clock.UtcNow,
                EventData = eventData,
                EventTypeCode = @event.EventTypeCode,
            };
            _logger.LogDebug("start to save event to store {@eventEntity}", eventEntity);

            await _oneTypeOneTableEventStoreProvider.InsertOneAsync(eventEntity);
        }

        public async Task SaveManyAsync(IEnumerable<IEvent> events)
        {
            var entities = events.Select(@event =>
            {
                var eventData =
                    _eventDataStringSerializer.Serialize(@event.ClaptrapIdentity.TypeCode, @event.EventTypeCode,
                        @event.Data);
                var eventEntity = new OneTypeOneTableEventEntity
                {
                    Version = @event.Version,
                    CreatedTime = _clock.UtcNow,
                    EventData = eventData,
                    EventTypeCode = @event.EventTypeCode,
                };
                return eventEntity;
            });

            await _oneTypeOneTableEventStoreProvider.InsertManyAsync(entities);
        }
    }
}