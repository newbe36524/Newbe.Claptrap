namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.OneIdentityOneTable
{
    public class OneTypeOneTableEventEntityMapper : IEventEntityMapper<OneIdentityOneTableEventEntity>
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

        public OneIdentityOneTableEventEntity Map(IEvent @event)
        {
            var eventData =
                _eventDataStringSerializer.Serialize(@event.ClaptrapIdentity.TypeCode, @event.EventTypeCode,
                    @event.Data);
            var eventEntity = new OneIdentityOneTableEventEntity
            {
                Version = @event.Version,
                CreatedTime = _clock.UtcNow,
                EventData = eventData,
                EventTypeCode = @event.EventTypeCode,
            };
            return eventEntity;
        }

        public IEvent Map(OneIdentityOneTableEventEntity entity, IClaptrapIdentity identity)
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