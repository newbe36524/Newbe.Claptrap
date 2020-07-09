using System;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite;
using Newbe.Claptrap.StorageProvider.SQLite.Extensions;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        /// <summary>
        /// it will be invoked form HostExtensions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="storageOptions"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder UseSQLiteAsEventStore(
            this IClaptrapBootstrapperBuilder builder,
            StorageOptions storageOptions)
            => builder.UseSQLite(sqlite =>
                sqlite.AsEventStore(eventStore =>
                {
                    switch (storageOptions.Strategy)
                    {
                        case RelationLocatorStrategy.SharedTable:
                            eventStore.SharedTable(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneTypeOneTable:
                            eventStore.OneTypeOneFile(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneIdOneTable:
                            eventStore.OneIdOneFile(ConfigMore);
                            break;
                        case RelationLocatorStrategy.Known:
                        default:
                            eventStore.UseLocator(new RelationalEventStoreLocator
                            {
                                ConnectionName = storageOptions.ConnectionName,
                                SchemaName = Consts.SQLiteSchemaName,
                                EventTableName = storageOptions.TableName,
                            }, ConfigMore);
                            break;
                    }

                    void ConfigMore(SQLiteEventStoreOptions sqliteOptions)
                    {
                        sqliteOptions.IsAutoMigrationEnabled = storageOptions.IsAutoMigrationEnabled;
                    }
                }));

        /// <summary>
        /// it will be invoked form HostExtensions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="storageOptions"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder UseSQLiteAsStateStore(
            this IClaptrapBootstrapperBuilder builder,
            StorageOptions storageOptions)
            => builder.UseSQLite(sqlite =>
                sqlite.AsStateStore(stateStore =>
                {
                    switch (storageOptions.Strategy)
                    {
                        case RelationLocatorStrategy.SharedTable:
                            stateStore.SharedTable(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneTypeOneTable:
                            stateStore.OneTypeOneFile(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneIdOneTable:
                            stateStore.OneIdOneFile(ConfigMore);
                            break;
                        case RelationLocatorStrategy.Known:
                        default:
                            stateStore.UseLocator(new RelationalStateStoreLocator
                            {
                                ConnectionName = storageOptions.ConnectionName,
                                // it is not support to change schema name in SQLite
                                SchemaName = Consts.SQLiteSchemaName,
                                StateTableName = storageOptions.TableName
                            }, ConfigMore);
                            break;
                    }

                    void ConfigMore(SQLiteStateStoreOptions options)
                    {
                        options.IsAutoMigrationEnabled = storageOptions.IsAutoMigrationEnabled;
                    }
                }));

        public static IClaptrapBootstrapperBuilder UseSQLite(
            this IClaptrapBootstrapperBuilder builder,
            Action<SQLiteProviderConfigurator> sqlite)
        {
            return builder.UseSQLite(x => true, sqlite);
        }

        public static IClaptrapBootstrapperBuilder UseSQLite(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<SQLiteProviderConfigurator> sqlite)
        {
            var sqLiteProviderConfigurator = new SQLiteProviderConfigurator(designFilter, builder);
            sqlite(sqLiteProviderConfigurator);
            return builder;
        }

        /// <summary>
        /// Use SQLite as storage in testing environment, it is almost ready for small unit test or integration test
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder UseSQLiteAsTestingStorage(
            this IClaptrapBootstrapperBuilder builder) =>
            builder.UseSQLite(sqlite =>
                sqlite
                    .AsEventStore(evenStore => evenStore.SharedTable())
                    .AsStateStore(stateStore => stateStore.SharedTable()));
    }
}