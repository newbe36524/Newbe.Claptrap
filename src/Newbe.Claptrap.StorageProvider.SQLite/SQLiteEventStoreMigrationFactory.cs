using System;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class SQLiteEventStoreMigrationFactory :
        IEventStoreMigrationFactory
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        private readonly OneIdentityOneTableDbUpSQLiteMigration.Factory
            _oneIdentityOneTableDbUpSQLiteMigrationFactory;

        public SQLiteEventStoreMigrationFactory(
            IClaptrapDesignStore claptrapDesignStore,
            OneIdentityOneTableDbUpSQLiteMigration.Factory
                oneIdentityOneTableDbUpSQLiteMigrationFactory)
        {
            _claptrapDesignStore = claptrapDesignStore;
            _oneIdentityOneTableDbUpSQLiteMigrationFactory =
                oneIdentityOneTableDbUpSQLiteMigrationFactory;
        }

        public IEventLoaderMigration CreateEventLoaderMigration(IClaptrapIdentity identity)
        {
            var claptrapDesign = _claptrapDesignStore.FindDesign(identity);
            var loaderOptions = claptrapDesign.StorageProviderOptions.EventLoaderOptions;
            var relationalEventLoaderOptions = (IRelationalEventLoaderOptions) loaderOptions;
            switch (relationalEventLoaderOptions.EventStoreStrategy)
            {
                case EventStoreStrategy.SharedTable:
                    break;
                case EventStoreStrategy.OneTypeOneTable:
                    break;
                case EventStoreStrategy.OneIdentityOneTable:
                    return _oneIdentityOneTableDbUpSQLiteMigrationFactory(
                        identity,
                        (ISQLiteOneIdentityOneTableEventStoreOptions) relationalEventLoaderOptions);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }

        public IEventSaverMigration CreateEventSaverMigration(IClaptrapIdentity identity)
        {
            var claptrapDesign = _claptrapDesignStore.FindDesign(identity);
            var saverOptions = claptrapDesign.StorageProviderOptions.EventSaverOptions;
            var relationalEventSaverOptions = (IRelationalEventSaverOptions) saverOptions;
            switch (relationalEventSaverOptions.EventStoreStrategy)
            {
                case EventStoreStrategy.SharedTable:
                    break;
                case EventStoreStrategy.OneTypeOneTable:
                    break;
                case EventStoreStrategy.OneIdentityOneTable:
                    return _oneIdentityOneTableDbUpSQLiteMigrationFactory(
                        identity,
                        (ISQLiteOneIdentityOneTableEventStoreOptions) relationalEventSaverOptions);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }
    }
}