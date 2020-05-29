namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class StateEntityMapper : IStateEntityMapper<StateEntity>
    {
        private readonly IClock _clock;
        private readonly IStateDataStringSerializer _stateDataStringSerializer;

        public StateEntityMapper(
            IClock clock,
            IStateDataStringSerializer stateDataStringSerializer)
        {
            _clock = clock;
            _stateDataStringSerializer = stateDataStringSerializer;
        }

        public StateEntity Map(IState stateEntity)
        {
            var stateData = _stateDataStringSerializer.Serialize(
                stateEntity.Identity.TypeCode,
                stateEntity.Data);
            var re = new StateEntity
            {
                Version = stateEntity.Version,
                StateData = stateData,
                UpdatedTime = _clock.UtcNow,
                ClaptrapId = stateEntity.Identity.Id,
                ClaptrapTypeCode = stateEntity.Identity.TypeCode,
            };
            return re;
        }

        public IState Map(StateEntity stateEntity, IClaptrapIdentity identity)
        {
            var stateData = _stateDataStringSerializer.Deserialize(
                identity.TypeCode, stateEntity.StateData);
            var re = new DataState(identity, stateData, stateEntity.Version);
            return re;
        }
    }
}