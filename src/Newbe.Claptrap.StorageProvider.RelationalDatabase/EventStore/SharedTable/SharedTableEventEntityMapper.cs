namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.SharedTable
{
    public class SharedTableEventEntityMapper : IEventEntityMapper<SharedTableEventEntity>
    {
        private readonly IClock _clock;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public SharedTableEventEntityMapper(
            IClock clock,
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _clock = clock;
            _eventDataStringSerializer = eventDataStringSerializer;
        }

        public SharedTableEventEntity Map(IEvent @event)
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
        }

        public IEvent Map(SharedTableEventEntity entity, IClaptrapIdentity identity)
        {
            var eventData =
                _eventDataStringSerializer.Deserialize(identity.TypeCode, entity.EventTypeCode, entity.EventData);
            var dataEvent = new DataEvent(identity, entity.EventTypeCode, eventData)
            {
                Version = entity.Version
            };
            return dataEvent;
        }
    }
}