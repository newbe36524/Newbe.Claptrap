using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable
{
    public interface ISharedTableEventStoreProvider
    {
        Task InsertOneAsync(SharedTableEventEntity entity);
        Task InsertManyAsync(IEnumerable<SharedTableEventEntity> entities);
        Task<IEnumerable<SharedTableEventEntity>> SelectAsync(long startVersion, long endVersion);
        IDbMigrationManager DbMigrationManager(IClaptrapIdentity identity);
    }
}