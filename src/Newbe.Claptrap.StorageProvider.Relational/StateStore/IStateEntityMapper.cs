namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public interface IStateEntityMapper<T>
        where T : IStateEntity
    {
        T Map(IState stateEntity);
        IState Map(T stateEntity, IClaptrapIdentity identity);
    }
}