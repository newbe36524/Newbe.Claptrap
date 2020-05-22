using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore
{
    public interface IEventEntitySaver<in T>
        where T : IEventEntity
    {
        Task SaveAsync(T eventEntity);
    }
}