using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public interface IEventEntitySaver<in T>
        where T : IEventEntity
    {
        Task SaveAsync(T entity);
    }
}