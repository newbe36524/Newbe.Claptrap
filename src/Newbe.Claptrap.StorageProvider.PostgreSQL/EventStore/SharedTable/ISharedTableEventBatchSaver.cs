using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore.SharedTable
{
    public interface ISharedTableEventBatchSaver
    {
        Task SaveAsync(EventEntity entity);
    }
}