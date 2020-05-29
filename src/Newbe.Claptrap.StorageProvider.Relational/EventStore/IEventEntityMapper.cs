namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public interface IEventEntityMapper<T>
        where T : IEventEntity

    {
        T Map(IEvent @event);
        IEvent Map(T entity, IClaptrapIdentity identity);
    }
}