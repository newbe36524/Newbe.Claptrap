namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.OneTypeOneTable
{
    public class OneTypeOneTableEventEntityMapper : IEventEntityMapper<OneTypeOneTableEventEntity>
    {
        private readonly IClock _clock;
        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public OneTypeOneTableEventEntityMapper(
            IClock clock,
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _clock = clock;
            _eventDataStringSerializer = eventDataStringSerializer;
        }

        public OneTypeOneTableEventEntity Map(IEvent @event)
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
                ClaptrapId = @event.ClaptrapIdentity.Id,
            };
            return eventEntity;
        }

        public IEvent Map(OneTypeOneTableEventEntity entity, IClaptrapIdentity identity)
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