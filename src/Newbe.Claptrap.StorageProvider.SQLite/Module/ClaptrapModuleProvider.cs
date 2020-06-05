using System;
using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options.Core;
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
                    builder.RegisterType<SQLiteRelationalEventEntityLoader>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.EventLoaderOptions,
                        typeof(SQLiteRelationalEventStoreMigration),
                        typeof(IEventLoaderMigration));
                }

                if (options.EventSaverOptions is ISQLiteEventSaverOptions)
                {
                    builder.RegisterType<SQLiteRelationalEventEntitySaver>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.EventLoaderOptions,
                        typeof(SQLiteRelationalEventStoreMigration),
                        typeof(IEventSaverMigration));
                }

                if (options.StateLoaderOptions is ISQLiteStateLoaderOptions)
                {
                    builder.RegisterType<SQLiteRelationalStateEntityLoader>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.StateLoaderOptions,
                        typeof(SQLiteRelationalStateStoreMigration),
                        typeof(IStateLoaderMigration));
                }

                if (options.StateSaverOptions is ISQLiteStateSaverOptions)
                {
                    builder.RegisterType<SQLiteRelationalStateEntitySaver>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.StateSaverOptions,
                        typeof(SQLiteRelationalStateStoreMigration),
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