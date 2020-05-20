using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public class SharedTableEventSaver : IEventSaver
    {
        private readonly ILogger<SharedTableEventSaver> _logger;
        private readonly IClock _clock;
        private readonly ISharedTableEventSaverProvider _sharedTableEventSaverProvider;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public SharedTableEventSaver(
            IClaptrapIdentity identity,
            ILogger<SharedTableEventSaver> logger,
            IClock clock,
            ISharedTableEventSaverProvider sharedTableEventSaverProvider,
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _eventDataStringSerializer = eventDataStringSerializer;
            Identity = identity;
            _logger = logger;
            _clock = clock;
            _sharedTableEventSaverProvider = sharedTableEventSaverProvider;
        }

        public IClaptrapIdentity Identity { get; }

        public async Task SaveEventAsync(IEvent @event)
        {
            try
            {
                await SaveEventAsyncCore();
            }
            catch (Exception e)
            {
                throw new EventSavingException(e, @event);
            }

            async Task SaveEventAsyncCore()
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
                await _sharedTableEventSaverProvider.SaveAsync(eventEntity);
            }
        }
    }
}