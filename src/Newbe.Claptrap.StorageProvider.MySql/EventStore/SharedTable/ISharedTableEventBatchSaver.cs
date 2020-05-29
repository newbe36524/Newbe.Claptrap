using System.Collections.Generic;
using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public interface ISharedTableEventBatchSaver
    {
        Task SaveManyAsync(IEnumerable<EventEntity> entities);
    }
}