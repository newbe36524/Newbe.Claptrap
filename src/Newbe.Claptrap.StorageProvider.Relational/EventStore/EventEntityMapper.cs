namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class EventEntityMapper : IEventEntityMapper<EventEntity>
    {
        private readonly IClock _clock;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public EventEntityMapper(
            IClock clock,
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _clock = clock;
            _eventDataStringSerializer = eventDataStringSerializer;
        }

        public EventEntity Map(IEvent @event)
        {
            var eventData =
                _eventDataStringSerializer.Serialize(@event.ClaptrapIdentity, @event.EventTypeCode, @event.Data);
            var eventEntity = new EventEntity
            {
                Version = @event.Version,
                CreatedTime = _clock.UtcNow,
                EventData = eventData,
                EventTypeCode = @event.EventTypeCode,
                ClaptrapId = @event.ClaptrapIdentity.Id,
                ClaptrapTypeCode = @event.ClaptrapIdentity.TypeCode
            };
            return eventEntity;
        }

        public IEvent Map(EventEntity entity, IClaptrapIdentity identity)
        {
            var eventData =
                _eventDataStringSerializer.Deserialize(identity, entity.EventTypeCode, entity.EventData);
            var dataEvent = new DataEvent(identity, entity.EventTypeCode, eventData)
            {
                Version = entity.Version
            };
            return dataEvent;
        }
    }
}