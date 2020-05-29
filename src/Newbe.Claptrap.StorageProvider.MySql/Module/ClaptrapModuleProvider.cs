using System;
using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.MySql.EventStore;
using Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.MySql.StateStore.SharedTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

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
                var options = _design.ClaptrapStorageProviderOptions;
                if (options.EventLoaderOptions is IMySqlEventLoaderOptions relationalEventLoaderOptions)
                {
                    switch (relationalEventLoaderOptions.MySqlEventStoreStrategy)
                    {
                        case MySqlEventStoreStrategy.SharedTable:
                            builder.RegisterType<MySqlSharedTableEventEntityLoader>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.EventLoaderOptions,
                                typeof(MySqlSharedTableEventStoreMigration),
                                typeof(IEventLoaderMigration));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.EventSaverOptions is IMySqlEventSaverOptions relationalEventSaverOptions)
                {
                    switch (relationalEventSaverOptions.MySqlEventStoreStrategy)
                    {
                        case MySqlEventStoreStrategy.SharedTable:
                            builder.RegisterType<MySqlSharedTableEventEntitySaver>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.EventSaverOptions,
                                typeof(MySqlSharedTableEventStoreMigration),
                                typeof(IEventSaverMigration));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.StateLoaderOptions is IMySqlStateLoaderOptions relationalStateLoaderOptions)
                {
                    switch (relationalStateLoaderOptions.MySqlStateStoreStrategy)
                    {
                        case MySqlStateStoreStrategy.SharedTable:
                            builder.RegisterType<MySqlSharedTableStateEntityLoader>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.StateLoaderOptions,
                                typeof(MySqlSharedTableStateStoreMigration),
                                typeof(IStateLoaderMigration));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (options.StateSaverOptions is IMySqlStateSaverOptions relationalStateSaverOptions)
                {
                    switch (relationalStateSaverOptions.MySqlStateStoreStrategy)
                    {
                        case MySqlStateStoreStrategy.SharedTable:
                            builder.RegisterType<MySqlSharedTableStateEntitySaver>()
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                            RegisterIfAutoMigrationEnabled(
                                options.StateSaverOptions,
                                typeof(MySqlSharedTableStateStoreMigration),
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
                    if (ops is IMySqlMigrationOptions
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