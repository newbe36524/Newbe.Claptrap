using System;
using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore;
using Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.PostgreSQL.StateStore.SharedTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Module
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
            var re = new PostgreSQLMigrationModule(design);
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

        private class PostgreSQLMigrationModule : Autofac.Module, IClaptrapSharedModule
        {
            private readonly IClaptrapDesign _design;

            public PostgreSQLMigrationModule(IClaptrapDesign design)
            {
                _design = design;
            }

            public string Name { get; } = "Claptrap PostgreSQL migration module";
            public string Description { get; } = "Module for PostgreSQL database migration";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                var options = _design.ClaptrapStorageProviderOptions;
                if (options.EventLoaderOptions is IPostgreSQLEventLoaderOptions relationalEventLoaderOptions)
                {
                    switch (relationalEventLoaderOptions.PostgreSQLEventStoreStrategy)
                    {
                        case PostgreSQLEventStoreStrategy.SharedTable:
                            builder.RegisterType<PostgreSQLSharedTableEventEntityLoader>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.EventLoaderOptions,
                                typeof(PostgreSQLSharedTableEventStoreMigration),
                                typeof(IEventLoaderMigration));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.EventSaverOptions is IPostgreSQLEventSaverOptions relationalEventSaverOptions)
                {
                    switch (relationalEventSaverOptions.PostgreSQLEventStoreStrategy)
                    {
                        case PostgreSQLEventStoreStrategy.SharedTable:
                            builder.RegisterType<PostgreSQLSharedTableEventEntitySaver>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.EventSaverOptions,
                                typeof(PostgreSQLSharedTableEventStoreMigration),
                                typeof(IEventSaverMigration));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.StateLoaderOptions is IPostgreSQLStateLoaderOptions relationalStateLoaderOptions)
                {
                    switch (relationalStateLoaderOptions.PostgreSQLStateStoreStrategy)
                    {
                        case PostgreSQLStateStoreStrategy.SharedTable:
                            builder.RegisterType<PostgreSQLSharedTableStateEntityLoader>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.StateLoaderOptions,
                                typeof(PostgreSQLSharedTableStateStoreMigration),
                                typeof(IStateLoaderMigration));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.StateSaverOptions is IPostgreSQLStateSaverOptions relationalStateSaverOptions)
                {
                    switch (relationalStateSaverOptions.PostgreSQLStateStoreStrategy)
                    {
                        case PostgreSQLStateStoreStrategy.SharedTable:
                            builder.RegisterType<PostgreSQLSharedTableStateEntitySaver>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.StateSaverOptions,
                                typeof(PostgreSQLSharedTableStateStoreMigration),
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
                    if (ops is IPostgreSQLMigrationOptions
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