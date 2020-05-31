using System;
using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.MongoDB.EventStore.SharedCollection;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.MongoDB.StateStore.SharedCollection;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Module
{
    public class ClaptrapModuleProvider : IClaptrapModuleProvider
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public ClaptrapModuleProvider(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public IEnumerable<IClaptrapSharedModule> GetClaptrapSharedModules(IClaptrapIdentity identity)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            var re = new MongoDBMigrationModule(design);
            yield return re;
        }

        public IEnumerable<IClaptrapMasterModule> GetClaptrapMasterClaptrapModules(IClaptrapIdentity identity)
        {
            yield break;
        }

        public IEnumerable<IClaptrapMinionModule> GetClaptrapMinionModules(IClaptrapIdentity identity)
        {
            yield break;
        }

        private class MongoDBMigrationModule : Autofac.Module, IClaptrapSharedModule
        {
            private readonly IClaptrapDesign _design;

            public MongoDBMigrationModule(IClaptrapDesign design)
            {
                _design = design;
            }

            public string Name { get; } = "Claptrap MongoDB migration module";
            public string Description { get; } = "Module for MongoDB database migration";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                var options = _design.ClaptrapStorageProviderOptions;
                if (options.EventLoaderOptions is IMongoDBEventLoaderOptions relationalEventLoaderOptions)
                {
                    switch (relationalEventLoaderOptions.MongoDBEventStoreStrategy)
                    {
                        case MongoDBEventStoreStrategy.SharedCollection:
                            builder.RegisterType<MongoDBSharedCollectionEventEntityLoader>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            builder.RegisterType<MongoDBSharedCollectionEventStoreMigration>()
                                .As<IEventLoaderMigration>()
                                .InstancePerLifetimeScope();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.EventSaverOptions is IMongoDBEventSaverOptions relationalEventSaverOptions)
                {
                    switch (relationalEventSaverOptions.MongoDBEventStoreStrategy)
                    {
                        case MongoDBEventStoreStrategy.SharedCollection:
                            builder.RegisterType<MongoDBSharedCollectionEventEntitySaver>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            builder.RegisterType<MongoDBSharedCollectionEventStoreMigration>()
                                .As<IEventSaverMigration>()
                                .InstancePerLifetimeScope();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.StateLoaderOptions is IMongoDBStateLoaderOptions relationalStateLoaderOptions)
                {
                    switch (relationalStateLoaderOptions.MongoDBStateStoreStrategy)
                    {
                        case MongoDBStateStoreStrategy.SharedCollection:
                            builder.RegisterType<MongoDBSharedCollectionStateEntityLoader>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            builder.RegisterType<MongoDBSharedCollectionStateStoreMigration>()
                                .As<IStateLoaderMigration>()
                                .InstancePerLifetimeScope();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.StateSaverOptions is IMongoDBStateSaverOptions relationalStateSaverOptions)
                {
                    switch (relationalStateSaverOptions.MongoDBStateStoreStrategy)
                    {
                        case MongoDBStateStoreStrategy.SharedCollection:
                            builder.RegisterType<MongoDBSharedCollectionStateEntitySaver>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            builder.RegisterType<MongoDBSharedCollectionStateStoreMigration>()
                                .As<IStateSaverMigration>()
                                .InstancePerLifetimeScope();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}