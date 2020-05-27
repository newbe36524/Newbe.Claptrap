using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Module
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
            var re = new SQLiteMigrationModule(design);
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

        private class SQLiteMigrationModule : Autofac.Module, IClaptrapSharedModule
        {
            private readonly IClaptrapDesign _design;

            public SQLiteMigrationModule(IClaptrapDesign design)
            {
                _design = design;
            }

            public string Name { get; } = "Claptrap SQLite migration module";
            public string Description { get; } = "Module for SQLite migration";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                builder.RegisterType<SQLiteEventStoreMigrationFactory>()
                    .As<IEventStoreMigrationFactory>()
                    .InstancePerLifetimeScope();
                if (_design.ClaptrapStorageProviderOptions.EventLoaderOptions is ISQLiteStorageMigrationOptions
                    eventLoaderMigrationOptions && eventLoaderMigrationOptions.IsAutoMigrationEnabled)
                {
                    builder.Register(t => t.Resolve<IEventStoreMigrationFactory>()
                            .CreateEventLoaderMigration(t.Resolve<IClaptrapIdentity>()))
                        .As<IEventLoaderMigration>()
                        .InstancePerLifetimeScope();
                }

                if (_design.ClaptrapStorageProviderOptions.EventSaverOptions is ISQLiteStorageMigrationOptions
                    eventSaverMigrationOptions && eventSaverMigrationOptions.IsAutoMigrationEnabled)
                {
                    builder.Register(t => t.Resolve<IEventStoreMigrationFactory>()
                            .CreateEventSaverMigration(t.Resolve<IClaptrapIdentity>()))
                        .As<IEventSaverMigration>()
                        .InstancePerLifetimeScope();
                }

                builder.RegisterType<OneIdentityOneTableDbUpSQLiteMigration>()
                    .AsSelf()
                    .InstancePerLifetimeScope();
            }
        }
    }
}