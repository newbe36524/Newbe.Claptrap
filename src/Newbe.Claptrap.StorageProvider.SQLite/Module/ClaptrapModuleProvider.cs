using System;
using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.SQLite.Options;
using Newbe.Claptrap.StorageProvider.SQLite.StateStore.OneIdentityOneTable;

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
                var options = _design.ClaptrapStorageProviderOptions;
                if (options.EventLoaderOptions is ISQLiteStorageMigrationOptions
                    eventLoaderMigrationOptions && eventLoaderMigrationOptions.IsAutoMigrationEnabled)
                {
                    if (options.EventLoaderOptions is IRelationalEventLoaderOptions relationalEventLoaderOptions)
                    {
                        switch (relationalEventLoaderOptions.EventStoreStrategy)
                        {
                            case EventStoreStrategy.SharedTable:
                                break;
                            case EventStoreStrategy.OneTypeOneTable:
                                break;
                            case EventStoreStrategy.OneIdentityOneTable:
                                builder.RegisterType<SQLiteOneIdentityOneTableEventStoreMigration>()
                                    .As<IEventLoaderMigration>()
                                    .InstancePerLifetimeScope();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                if (options.EventSaverOptions is ISQLiteStorageMigrationOptions
                        eventSaverMigrationOptions && eventSaverMigrationOptions.IsAutoMigrationEnabled
                                                   && options.EventSaverOptions is IRelationalEventSaverOptions
                                                       relationalEventSaverOptions)
                {
                    switch (relationalEventSaverOptions.EventStoreStrategy)
                    {
                        case EventStoreStrategy.SharedTable:
                            break;
                        case EventStoreStrategy.OneTypeOneTable:
                            break;
                        case EventStoreStrategy.OneIdentityOneTable:
                            builder.RegisterType<SQLiteOneIdentityOneTableEventStoreMigration>()
                                .As<IEventSaverMigration>()
                                .InstancePerLifetimeScope();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.StateLoaderOptions is ISQLiteStorageMigrationOptions
                    stateLoaderMigrationOptions && stateLoaderMigrationOptions.IsAutoMigrationEnabled)
                {
                    if (options.StateLoaderOptions is IRelationalStateLoaderOptions relationalStateLoaderOptions)
                    {
                        switch (relationalStateLoaderOptions.StateStoreStrategy)
                        {
                            case StateStoreStrategy.OneIdentityOneTable:
                                builder.RegisterType<SQLiteOneIdentityOneTableStateStoreMigration>()
                                    .As<IStateLoaderMigration>()
                                    .InstancePerLifetimeScope();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                if (options.StateSaverOptions is ISQLiteStorageMigrationOptions
                    stateSaverMigrationOptions && stateSaverMigrationOptions.IsAutoMigrationEnabled)
                {
                    if (options.StateSaverOptions is IRelationalStateSaverOptions relationalStateSaverOptions)
                    {
                        switch (relationalStateSaverOptions.StateStoreStrategy)
                        {
                            case StateStoreStrategy.OneIdentityOneTable:
                                builder.RegisterType<SQLiteOneIdentityOneTableStateStoreMigration>()
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
}