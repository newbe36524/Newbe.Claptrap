using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore.SharedCollection
{
    public interface ISharedCollectionEventBatchSaver
    {
        Task SaveAsync(EventEntity entity);
    }
}