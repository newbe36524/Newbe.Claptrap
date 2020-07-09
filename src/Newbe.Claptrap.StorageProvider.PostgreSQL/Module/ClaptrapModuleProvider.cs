using System;
using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.PostgreSQL.StateStore;
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
                if (options.EventLoaderOptions is IPostgreSQLEventLoaderOptions)
                {
                    builder.RegisterType<PostgreSQLEventEntityLoader>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.EventLoaderOptions,
                        typeof(PostgreSQLEventStoreMigration),
                        typeof(IEventLoaderMigration));
                }

                if (options.EventSaverOptions is IPostgreSQLEventSaverOptions)
                {
                    builder.RegisterType<PostgreSQLEventEntitySaver>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.EventSaverOptions,
                        typeof(PostgreSQLEventStoreMigration),
                        typeof(IEventSaverMigration));
                }

                if (options.StateLoaderOptions is IPostgreSQLStateLoaderOptions)
                {
                    builder.RegisterType<PostgreSQLStateEntityLoader>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.StateLoaderOptions,
                        typeof(PostgreSQLStateStoreMigration),
                        typeof(IStateLoaderMigration));
                }

                if (options.StateSaverOptions is IPostgreSQLStateSaverOptions)
                {
                    builder.RegisterType<PostgreSQLStateEntitySaver>()
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope();
                    RegisterIfAutoMigrationEnabled(
                        options.StateSaverOptions,
                        typeof(PostgreSQLStateStoreMigration),
                        typeof(IStateSaverMigration));
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