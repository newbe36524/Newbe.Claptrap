using System;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Extensions;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

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
        public static IClaptrapBootstrapperBuilder UsePostgreSQLAsEventStore(
            this IClaptrapBootstrapperBuilder builder,
            StorageOptions storageOptions)
            => builder.UsePostgreSQL(sqlite =>
                sqlite.AsEventStore(eventStore =>
                {
                    switch (storageOptions.Strategy)
                    {
                        case RelationLocatorStrategy.SharedTable:
                            eventStore.SharedTable(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneTypeOneTable:
                            eventStore.OneTypeOneTable(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneIdOneTable:
                            eventStore.OneIdOneTable(ConfigMore);
                            break;
                        case RelationLocatorStrategy.Known:
                        default:
                            eventStore.UseLocator(new RelationalEventStoreLocator
                            {
                                ConnectionName = storageOptions.ConnectionName,
                                SchemaName = storageOptions.SchemaName,
                                EventTableName = storageOptions.TableName,
                            }, ConfigMore);
                            break;
                    }

                    void ConfigMore(PostgreSQLEventStoreOptions options)
                    {
                        options.IsAutoMigrationEnabled = storageOptions.IsAutoMigrationEnabled;
                        if (storageOptions.InsertManyWindowTimeInMilliseconds.HasValue)
                        {
                            options.InsertManyWindowTimeInMilliseconds =
                                storageOptions.InsertManyWindowTimeInMilliseconds;
                        }

                        if (storageOptions.InsertManyWindowCount.HasValue)
                        {
                            options.InsertManyWindowCount = storageOptions.InsertManyWindowCount;
                        }
                    }
                }));

        /// <summary>
        /// it will be invoked form HostExtensions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="storageOptions"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder UsePostgreSQLAsStateStore(
            this IClaptrapBootstrapperBuilder builder,
            StorageOptions storageOptions)
            => builder.UsePostgreSQL(sqlite =>
                sqlite.AsStateStore(stateStore =>
                {
                    switch (storageOptions.Strategy)
                    {
                        case RelationLocatorStrategy.SharedTable:
                            stateStore.SharedTable(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneTypeOneTable:
                            stateStore.OneTypeOneTable(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneIdOneTable:
                            stateStore.OneIdOneTable(ConfigMore);
                            break;
                        case RelationLocatorStrategy.Known:
                        default:
                            stateStore.UseLocator(new RelationalStateStoreLocator
                            {
                                ConnectionName = storageOptions.ConnectionName,
                                SchemaName = storageOptions.SchemaName,
                                StateTableName = storageOptions.TableName
                            }, ConfigMore);
                            break;
                    }

                    void ConfigMore(PostgreSQLStateStoreOptions options)
                    {
                        options.IsAutoMigrationEnabled = storageOptions.IsAutoMigrationEnabled;
                        options.InsertManyWindowTimeInMilliseconds =
                            storageOptions.InsertManyWindowTimeInMilliseconds;
                        options.InsertManyWindowCount = storageOptions.InsertManyWindowCount;
                    }
                }));

        public static IClaptrapBootstrapperBuilder UsePostgreSQL(
            this IClaptrapBootstrapperBuilder builder,
            Action<PostgreSQLProviderConfigurator> postgreSQL)
        {
            return builder.UsePostgreSQL(x => true, postgreSQL);
        }

        public static IClaptrapBootstrapperBuilder UsePostgreSQL(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<PostgreSQLProviderConfigurator> postgreSQL)
        {
            var configurator = new PostgreSQLProviderConfigurator(designFilter, builder);
            postgreSQL(configurator);
            return builder;
        }
    }
}