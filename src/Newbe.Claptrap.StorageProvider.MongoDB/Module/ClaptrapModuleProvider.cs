using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.MongoDB.EventStore;
using Newbe.Claptrap.StorageProvider.MongoDB.Options;
using Newbe.Claptrap.StorageProvider.MongoDB.StateStore;
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
                if (options.EventLoaderOptions is IMongoDBEventLoaderOptions)
                {
                    builder.RegisterType<MongoDBEventEntityLoader>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    builder.RegisterType<MongoDBEventStoreMigration>()
                        .As<IEventLoaderMigration>()
                        .InstancePerLifetimeScope();
                }

                if (options.EventSaverOptions is IMongoDBEventSaverOptions)
                {
                    builder.RegisterType<MongoDBEventEntitySaver>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    builder.RegisterType<MongoDBEventStoreMigration>()
                        .As<IEventSaverMigration>()
                        .InstancePerLifetimeScope();
                }

                if (options.StateLoaderOptions is IMongoDBStateLoaderOptions)
                {
                    builder.RegisterType<MongoDBStateEntityLoader>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    builder.RegisterType<MongoDBStateStoreMigration>()
                        .As<IStateLoaderMigration>()
                        .InstancePerLifetimeScope();
                }

                if (options.StateSaverOptions is IMongoDBStateSaverOptions)
                {
                    builder.RegisterType<MongoDBStateEntitySaver>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    builder.RegisterType<MongoDBStateStoreMigration>()
                        .As<IStateSaverMigration>()
                        .InstancePerLifetimeScope();
                }
            }
        }
    }
}