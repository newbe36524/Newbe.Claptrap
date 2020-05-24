using Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public interface IEventStoreMigrationManagerFactory
    {
        IEventStoreMigrationManager Create(IClaptrapIdentity identity);
    }

    public class EventStoreMigrationManagerFactory : IEventStoreMigrationManagerFactory
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        private readonly SharedTableEventStoreDbUpMysqlMigrationManager.Factory
            _sharedTableEventStoreDbUpMysqlMigrationManagerFactory;

        public EventStoreMigrationManagerFactory(
            IClaptrapDesignStore claptrapDesignStore,
            SharedTableEventStoreDbUpMysqlMigrationManager.Factory
                sharedTableEventStoreDbUpMysqlMigrationManagerFactory)
        {
            _claptrapDesignStore = claptrapDesignStore;
            _sharedTableEventStoreDbUpMysqlMigrationManagerFactory =
                sharedTableEventStoreDbUpMysqlMigrationManagerFactory;
        }

        public IEventStoreMigrationManager Create(IClaptrapIdentity identity)
        {
            // TODO
            return _sharedTableEventStoreDbUpMysqlMigrationManagerFactory();
        }
    }
}