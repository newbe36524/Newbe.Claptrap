using System;
using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.OneTypeOneTable;
using Newbe.Claptrap.StorageProvider.Relational.EventStore.SharedTable;
using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.Relational.Module
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
            yield return new RelationalEventLoaderModule(design);
        }

        public IEnumerable<IClaptrapMasterModule> GetClaptrapMasterClaptrapModules(IClaptrapIdentity identity)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            yield return new RelationalEventSaverModule(design);
        }

        public IEnumerable<IClaptrapMinionModule> GetClaptrapMinionModules(IClaptrapIdentity identity)
        {
            yield break;
        }

        private class RelationalEventSaverModule : Autofac.Module, IClaptrapMasterModule
        {
            private readonly IClaptrapDesign _design;

            public RelationalEventSaverModule(IClaptrapDesign design)
            {
                _design = design;
            }

            public string Name { get; } =
                "Claptrap relational database storage provider shared components module for IEventSaver";

            public string Description { get; } =
                "Module for claptrap relational database storage provider shared components for IEventSaver";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                var saverOptions = _design.ClaptrapStorageProviderOptions.EventSaverOptions;
                if (saverOptions is IRelationalEventSaverOptions relationalEventSaverOptions)
                {
                    if (saverOptions is IAutoMigrationOptions autoMigrationOptions
                        && autoMigrationOptions.IsAutoMigrationEnabled)
                    {
                        builder.RegisterType<AutoMigrationEventSaver>()
                            .AsSelf()
                            .InstancePerLifetimeScope();
                        builder.RegisterDecorator<IEventSaver>((context, ps, inner) => context
                            .Resolve<AutoMigrationEventSaver.Factory>()
                            .Invoke(inner));
                    }

                    switch (relationalEventSaverOptions.EventStoreStrategy)
                    {
                        case EventStoreStrategy.SharedTable:
                            builder.RegisterType<RelationalEventSaver<SharedTableEventEntity>>()
                                .As<IRelationalEventSaver>()
                                .InstancePerLifetimeScope();
                            break;
                        case EventStoreStrategy.OneTypeOneTable:
                            builder.RegisterType<RelationalEventSaver<OneTypeOneTableEventEntity>>()
                                .As<IRelationalEventSaver>()
                                .InstancePerLifetimeScope();
                            break;
                        case EventStoreStrategy.OneIdentityOneTable:
                            builder.RegisterType<RelationalEventSaver<OneIdentityOneTableEventEntity>>()
                                .As<IRelationalEventSaver>()
                                .InstancePerLifetimeScope();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private class RelationalEventLoaderModule : Autofac.Module, IClaptrapSharedModule
        {
            private readonly IClaptrapDesign _design;

            public RelationalEventLoaderModule(IClaptrapDesign design)
            {
                _design = design;
            }

            public string Name { get; } =
                "Claptrap relational database storage provider shared components module for IEventLoader";

            public string Description { get; } =
                "Module for claptrap relational database storage provider shared components for IEventLoader";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                var loaderOptions = _design.ClaptrapStorageProviderOptions.EventLoaderOptions;
                if (loaderOptions is IRelationalEventLoaderOptions relationalEventLoaderOptions)
                {
                    if (loaderOptions is IAutoMigrationOptions autoMigrationOptions
                        && autoMigrationOptions.IsAutoMigrationEnabled)
                    {
                        builder.RegisterType<AutoMigrationEventLoader>()
                            .AsSelf()
                            .InstancePerLifetimeScope();
                        builder.RegisterDecorator<IEventLoader>((context, ps, inner) => context
                            .Resolve<AutoMigrationEventLoader.Factory>()
                            .Invoke(inner));
                    }

                    switch (relationalEventLoaderOptions.EventStoreStrategy)
                    {
                        case EventStoreStrategy.SharedTable:
                            builder.RegisterType<RelationalEventLoader<SharedTableEventEntity>>()
                                .As<IRelationalEventLoader>()
                                .InstancePerLifetimeScope();
                            break;
                        case EventStoreStrategy.OneTypeOneTable:
                            builder.RegisterType<RelationalEventLoader<OneTypeOneTableEventEntity>>()
                                .As<IRelationalEventLoader>()
                                .InstancePerLifetimeScope();
                            break;
                        case EventStoreStrategy.OneIdentityOneTable:
                            builder.RegisterType<RelationalEventLoader<OneIdentityOneTableEventEntity>>()
                                .As<IRelationalEventLoader>()
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