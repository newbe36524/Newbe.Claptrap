using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.Module
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
            var re = new MySqlMigrationModule(design);
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

        private class MySqlMigrationModule : Autofac.Module, IClaptrapSharedModule
        {
            private readonly IClaptrapDesign _design;

            public MySqlMigrationModule(IClaptrapDesign design)
            {
                _design = design;
            }

            public string Name { get; } = "Claptrap mysql migration module";
            public string Description { get; } = "Module for mysql database migration";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                builder.RegisterType<MySqlEventStoreMigrationFactory>()
                    .As<IEventStoreMigrationFactory>()
                    .InstancePerLifetimeScope();
                if (_design.ClaptrapStorageProviderOptions.EventLoaderOptions is IMySqlMigrationOptions
                        eventLoaderMigrationOptions
                    && eventLoaderMigrationOptions.IsAutoMigrationEnabled)
                {
                    builder.Register(t => t.Resolve<IEventStoreMigrationFactory>()
                            .CreateEventLoaderMigration(t.Resolve<IClaptrapIdentity>()))
                        .As<IEventLoaderMigration>()
                        .InstancePerLifetimeScope();
                }

                if (_design.ClaptrapStorageProviderOptions.StateSaverOptions is IMySqlMigrationOptions
                        eventSaverMigrationOptions
                    && eventSaverMigrationOptions.IsAutoMigrationEnabled)
                {
                    builder.Register(t => t.Resolve<IEventStoreMigrationFactory>()
                            .CreateEventSaverMigration(t.Resolve<IClaptrapIdentity>()))
                        .As<IEventSaverMigration>()
                        .InstancePerLifetimeScope();
                }


                builder.RegisterType<SharedTableEventStoreDbUpMysqlMigration>()
                    .AsSelf()
                    .InstancePerLifetimeScope();
            }
        }
    }
}