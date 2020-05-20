using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.OneTypeOneTable
{
    public interface IOneTypeOneTableEventStoreProvider
    {
        string ClaptrapTypeCode { get; }
        Task InsertOneAsync(OneTypeOneTableEventEntity entity);
        Task InsertManyAsync(IEnumerable<OneTypeOneTableEventEntity> entities);
        Task<IEnumerable<OneTypeOneTableEventEntity>> SelectAsync(long startVersion, long endVersion);
        IDbMigrationManager DbMigrationManager(IClaptrapIdentity identity);
    }
}