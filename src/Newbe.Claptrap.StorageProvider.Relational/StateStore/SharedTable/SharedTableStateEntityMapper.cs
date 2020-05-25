namespace Newbe.Claptrap.StorageProvider.Relational.StateStore.SharedTable
{
    public class SharedTableStateEntityMapper : IStateEntityMapper<SharedTableStateEntity>
    {
        private readonly IClock _clock;
        private readonly IStateDataStringSerializer _stateDataStringSerializer;

        public SharedTableStateEntityMapper(
            IClock clock,
            IStateDataStringSerializer stateDataStringSerializer)
        {
            _clock = clock;
            _stateDataStringSerializer = stateDataStringSerializer;
        }

        public SharedTableStateEntity Map(IState stateEntity)
        {
            var stateData = _stateDataStringSerializer.Serialize(
                stateEntity.Identity.TypeCode,
                stateEntity.Data);
            var re = new SharedTableStateEntity
            {
                Version = stateEntity.Version,
                ClaptrapId = stateEntity.Identity.Id,
                StateData = stateData,
                UpdatedTime = _clock.UtcNow,
                ClaptrapTypeCode = stateEntity.Identity.TypeCode
            };
            return re;
        }

        public IState Map(SharedTableStateEntity stateEntity, IClaptrapIdentity identity)
        {
            var stateData = _stateDataStringSerializer.Deserialize(
                identity.TypeCode, stateEntity.StateData);
            var re = new DataState(identity, stateData, stateEntity.Version);
            return re;
        }
    }
}