namespace Newbe.Claptrap.StorageProvider.Relational.StateStore.OneIdentityOneTable
{
    public class OneIdentityOneTableStateEntityMapper : IStateEntityMapper<OneIdentityOneTableStateEntity>
    {
        private readonly IClock _clock;
        private readonly IStateDataStringSerializer _stateDataStringSerializer;

        public OneIdentityOneTableStateEntityMapper(
            IClock clock,
            IStateDataStringSerializer stateDataStringSerializer)
        {
            _clock = clock;
            _stateDataStringSerializer = stateDataStringSerializer;
        }

        public OneIdentityOneTableStateEntity Map(IState stateEntity)
        {
            var stateData = _stateDataStringSerializer.Serialize(
                stateEntity.Identity.TypeCode,
                stateEntity.Data);
            var re = new OneIdentityOneTableStateEntity
            {
                Version = stateEntity.Version,
                StateData = stateData,
                UpdatedTime = _clock.UtcNow,
                ClaptrapId = stateEntity.Identity.Id,
                ClaptrapTypeCode = stateEntity.Identity.TypeCode,
            };
            return re;
        }

        public IState Map(OneIdentityOneTableStateEntity stateEntity, IClaptrapIdentity identity)
        {
            var stateData = _stateDataStringSerializer.Deserialize(
                identity.TypeCode, stateEntity.StateData);
            var re = new DataState(identity, stateData, stateEntity.Version);
            return re;
        }
    }
}