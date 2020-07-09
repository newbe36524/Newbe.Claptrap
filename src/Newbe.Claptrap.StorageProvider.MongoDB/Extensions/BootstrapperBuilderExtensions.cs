using System;
using Newbe.Claptrap.StorageProvider.MongoDB.EventStore;
using Newbe.Claptrap.StorageProvider.MongoDB.Extensions;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.MongoDB.StateStore;

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
        public static IClaptrapBootstrapperBuilder UseMongoDBAsEventStore(
            this IClaptrapBootstrapperBuilder builder,
            StorageOptions storageOptions)
            => builder.UseMongoDB(sqlite =>
                sqlite.AsEventStore(eventStore =>
                {
                    switch (storageOptions.Strategy)
                    {
                        case RelationLocatorStrategy.SharedTable:
                            eventStore.SharedCollection(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneTypeOneTable:
                            eventStore.OneTypeOneCollection(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneIdOneTable:
                            eventStore.OneIdOneCollection(ConfigMore);
                            break;
                        case RelationLocatorStrategy.Known:
                        default:
                            eventStore.UseLocator(new MongoDBEventStoreLocator
                            {
                                ConnectionName = storageOptions.ConnectionName,
                                DatabaseName = storageOptions.SchemaName,
                                EventCollectionName = storageOptions.TableName,
                            }, ConfigMore);
                            break;
                    }

                    void ConfigMore(MongoDBEventStoreOptions options)
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
        public static IClaptrapBootstrapperBuilder UseMongoDBAsStateStore(
            this IClaptrapBootstrapperBuilder builder,
            StorageOptions storageOptions)
            => builder.UseMongoDB(sqlite =>
                sqlite.AsStateStore(stateStore =>
                {
                    switch (storageOptions.Strategy)
                    {
                        case RelationLocatorStrategy.SharedTable:
                            stateStore.SharedCollection(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneTypeOneTable:
                            stateStore.OneTypeOneCollection(ConfigMore);
                            break;
                        case RelationLocatorStrategy.OneIdOneTable:
                            stateStore.OneIdOneCollection(ConfigMore);
                            break;
                        case RelationLocatorStrategy.Known:
                        default:
                            stateStore.UseLocator(new MongoDBStateStoreLocator
                            {
                                ConnectionName = storageOptions.ConnectionName,
                                DatabaseName = storageOptions.SchemaName,
                                StateCollectionName = storageOptions.TableName
                            }, ConfigMore);
                            break;
                    }

                    void ConfigMore(MongoDBStateStoreOptions options)
                    {
                        options.IsAutoMigrationEnabled = storageOptions.IsAutoMigrationEnabled;
                        options.InsertManyWindowTimeInMilliseconds = storageOptions.InsertManyWindowTimeInMilliseconds;
                        options.InsertManyWindowCount = storageOptions.InsertManyWindowCount;
                    }
                }));

        public static IClaptrapBootstrapperBuilder UseMongoDB(
            this IClaptrapBootstrapperBuilder builder,
            Action<MongoDBProviderConfigurator> mongoDB)
        {
            return builder.UseMongoDB(x => true, mongoDB);
        }

        public static IClaptrapBootstrapperBuilder UseMongoDB(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<MongoDBProviderConfigurator> mongoDB)
        {
            var configurator = new MongoDBProviderConfigurator(designFilter, builder);
            mongoDB(configurator);
            return builder;
        }
    }
}