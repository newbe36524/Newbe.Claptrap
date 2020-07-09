using System;
using Newbe.Claptrap.StorageProvider.MySql.Extensions;
using Newbe.Claptrap.StorageProvider.MySql.Options;
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
        public static IClaptrapBootstrapperBuilder UseMySqlAsEventStore(
            this IClaptrapBootstrapperBuilder builder,
            StorageOptions storageOptions)
            => builder.UseMySql(sqlite =>
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

                    void ConfigMore(MySqlEventStoreOptions options)
                    {
                        options.IsAutoMigrationEnabled = storageOptions.IsAutoMigrationEnabled;
                        options.InsertManyWindowTimeInMilliseconds =
                            storageOptions.InsertManyWindowTimeInMilliseconds;
                        options.InsertManyWindowCount = storageOptions.InsertManyWindowCount;
                    }
                }));

        /// <summary>
        /// it will be invoked form HostExtensions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="storageOptions"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder UseMySqlAsStateStore(
            this IClaptrapBootstrapperBuilder builder,
            StorageOptions storageOptions)
            => builder.UseMySql(sqlite =>
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

                    void ConfigMore(MySqlStateStoreOptions options)
                    {
                        options.IsAutoMigrationEnabled = storageOptions.IsAutoMigrationEnabled;
                        options.InsertManyWindowTimeInMilliseconds =
                            storageOptions.InsertManyWindowTimeInMilliseconds;
                        options.InsertManyWindowCount = storageOptions.InsertManyWindowCount;
                    }
                }));
        
        public static IClaptrapBootstrapperBuilder UseMySql(
            this IClaptrapBootstrapperBuilder builder,
            Action<MySqlProviderConfigurator> mysql)
        {
            return builder.UseMySql(x => true, mysql);
        }

        public static IClaptrapBootstrapperBuilder UseMySql(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<MySqlProviderConfigurator> mysql)
        {
            var configurator = new MySqlProviderConfigurator(designFilter, builder);
            mysql(configurator);
            return builder;
        }
    }
}