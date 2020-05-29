using System;
using Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlEventStoreMigrationFactory : IEventStoreMigrationFactory
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        private readonly SharedTableEventStoreDbUpMysqlMigration.Factory
            _sharedTableEventStoreDbUpMysqlMigrationManagerFactory;

        public MySqlEventStoreMigrationFactory(
            IClaptrapDesignStore claptrapDesignStore,
            SharedTableEventStoreDbUpMysqlMigration.Factory
                sharedTableEventStoreDbUpMysqlMigrationManagerFactory)
        {
            _claptrapDesignStore = claptrapDesignStore;
            _sharedTableEventStoreDbUpMysqlMigrationManagerFactory =
                sharedTableEventStoreDbUpMysqlMigrationManagerFactory;
        }

        public IEventLoaderMigration CreateEventLoaderMigration(IClaptrapIdentity identity)
        {
            var claptrapDesign = _claptrapDesignStore.FindDesign(identity);
            var loaderOptions = claptrapDesign.ClaptrapStorageProviderOptions.EventLoaderOptions;
            var relationalEventLoaderOptions = (IRelationalEventLoaderOptions) loaderOptions;
            switch (relationalEventLoaderOptions.EventStoreStrategy)
            {
                case EventStoreStrategy.SharedTable:
                    return _sharedTableEventStoreDbUpMysqlMigrationManagerFactory(
                        (IMySqlSharedTableEventStoreOptions) relationalEventLoaderOptions);
                case EventStoreStrategy.OneTypeOneTable:
                    break;
                case EventStoreStrategy.OneIdentityOneTable:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }

        public IEventSaverMigration CreateEventSaverMigration(IClaptrapIdentity identity)
        {
            throw new System.NotImplementedException();
        }
    }
}