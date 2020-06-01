using System;
using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdOneFile;
using Newbe.Claptrap.StorageProvider.SQLite.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.SQLite.Options.Core;
using Newbe.Claptrap.StorageProvider.SQLite.StateStore.OneIdOneFile;
using Newbe.Claptrap.StorageProvider.SQLite.StateStore.SharedTable;

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
                if (options.EventLoaderOptions is ISQLiteEventLoaderOptions relationalEventLoaderOptions)
                {
                    switch (relationalEventLoaderOptions.SQLiteEventStoreStrategy)
                    {
                        case SQLiteEventStoreStrategy.OneIdOneFile:
                            builder.RegisterType<SQLiteOneIdOneFileEventEntityLoader>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.EventLoaderOptions,
                                typeof(SQLiteOneIdOneFileEventStoreMigration),
                                typeof(IEventLoaderMigration));
                            break;
                        case SQLiteEventStoreStrategy.SharedTable:
                            builder.RegisterType<SQLiteSharedTableEventEntityLoader>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.EventLoaderOptions,
                                typeof(SQLiteSharedTableEventStoreMigration),
                                typeof(IEventLoaderMigration));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.EventSaverOptions is ISQLiteEventSaverOptions relationalEventSaverOptions)
                {
                    switch (relationalEventSaverOptions.SQLiteEventStoreStrategy)
                    {
                        case SQLiteEventStoreStrategy.OneIdOneFile:
                            builder.RegisterType<SQLiteOneIdOneFileEventEntitySaver>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.EventSaverOptions,
                                typeof(SQLiteOneIdOneFileEventStoreMigration),
                                typeof(IEventSaverMigration));
                            break;
                        case SQLiteEventStoreStrategy.SharedTable:
                            builder.RegisterType<SQLiteSharedTableEventEntitySaver>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.EventLoaderOptions,
                                typeof(SQLiteSharedTableEventStoreMigration),
                                typeof(IEventSaverMigration));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.StateLoaderOptions is ISQLiteStateLoaderOptions relationalStateLoaderOptions)
                {
                    switch (relationalStateLoaderOptions.SQLiteStateStoreStrategy)
                    {
                        case SQLiteStateStoreStrategy.OneIdOneFile:
                            builder.RegisterType<SQLiteOneIdOneFileStateEntityLoader>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.StateLoaderOptions,
                                typeof(SQLiteOneIdOneFileStateStoreMigration),
                                typeof(IStateLoaderMigration));
                            break;
                        case SQLiteStateStoreStrategy.SharedTable:
                            builder.RegisterType<SQLiteSharedTableStateEntityLoader>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.StateLoaderOptions,
                                typeof(SQLiteSharedTableStateStoreMigration),
                                typeof(IStateLoaderMigration));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.StateSaverOptions is ISQLiteStateSaverOptions relationalStateSaverOptions)
                {
                    switch (relationalStateSaverOptions.SQLiteStateStoreStrategy)
                    {
                        case SQLiteStateStoreStrategy.OneIdOneFile:
                            builder.RegisterType<SQLiteOneIdOneFileStateEntitySaver>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.StateSaverOptions,
                                typeof(SQLiteOneIdOneFileStateStoreMigration),
                                typeof(IStateSaverMigration));
                            break;
                        case SQLiteStateStoreStrategy.SharedTable:
                            builder.RegisterType<SQLiteSharedTableStateEntitySaver>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.StateSaverOptions,
                                typeof(SQLiteSharedTableStateStoreMigration),
                                typeof(IStateSaverMigration));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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