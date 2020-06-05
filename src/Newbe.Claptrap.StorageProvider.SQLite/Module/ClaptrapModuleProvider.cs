using System;
using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;
using Newbe.Claptrap.StorageProvider.SQLite.StateStore;

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
            var re = new SQLiteStorageModule(design);
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

        private class SQLiteStorageModule : Autofac.Module, IClaptrapSharedModule
        {
            private readonly IClaptrapDesign _design;

            public SQLiteStorageModule(IClaptrapDesign design)
            {
                _design = design;
            }

            public string Name { get; } = "Claptrap SQLite module";
            public string Description { get; } = "Module for SQLite to support EventStore and StateStore";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                var options = _design.ClaptrapStorageProviderOptions;
                if (options.EventLoaderOptions is ISQLiteEventLoaderOptions)
                {
                    builder.RegisterType<SQLiteEventEntityLoader>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.EventLoaderOptions,
                        typeof(SQLiteEventStoreMigration),
                        typeof(IEventLoaderMigration));
                }

                if (options.EventSaverOptions is ISQLiteEventSaverOptions)
                {
                    builder.RegisterType<SQLiteEventEntitySaver>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.EventLoaderOptions,
                        typeof(SQLiteEventStoreMigration),
                        typeof(IEventSaverMigration));
                }

                if (options.StateLoaderOptions is ISQLiteStateLoaderOptions)
                {
                    builder.RegisterType<SQLiteStateEntityLoader>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.StateLoaderOptions,
                        typeof(SQLiteStateStoreMigration),
                        typeof(IStateLoaderMigration));
                }

                if (options.StateSaverOptions is ISQLiteStateSaverOptions)
                {
                    builder.RegisterType<SQLiteStateEntitySaver>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.StateSaverOptions,
                        typeof(SQLiteStateStoreMigration),
                        typeof(IStateSaverMigration));
                }

                void RegisterIfAutoMigrationEnabled(IStorageProviderOptions ops,
                    Type type,
                    Type migrationInterfaceType)
                {
                    if (ops is ISQLiteStorageMigrationOptions
                        migrationOptions && migrationOptions.IsAutoMigrationEnabled)
                    {
                        builder.RegisterType(type)
                            .As(migrationInterfaceType)
                            .InstancePerLifetimeScope();
                    }
                }
            }
        }
    }
}