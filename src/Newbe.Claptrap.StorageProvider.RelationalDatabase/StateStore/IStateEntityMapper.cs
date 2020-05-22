namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.StateStore
{
    public interface IStateEntityMapper<T>
        where T : IStateEntity
    {
        T Map(IState stateEntity);
        IState Map(T stateEntity, IClaptrapIdentity identity);
    }
}