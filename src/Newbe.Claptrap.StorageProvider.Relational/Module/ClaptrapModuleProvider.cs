using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.AppMetrics;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Options;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

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
            yield return new RelationalStateLoaderModule(design);
            yield return new RelationalStateSaverModule(design);
            yield return new AppMetricsModule();
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

        private class AppMetricsModule : Autofac.Module, IClaptrapSharedModule
        {
            public string Name { get; } = "Claptrap Metrics module";
            public string Description { get; } = "Module for metrics about EventStoreMigration and StateStoreMigration";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                builder.RegisterDecorator<MetricsEventLoaderMigration, IEventLoaderMigration>();
                builder.RegisterDecorator<MetricsEventSaverMigration, IEventSaverMigration>();
                builder.RegisterDecorator<MetricsStateLoaderMigration, IStateLoaderMigration>();
                builder.RegisterDecorator<MetricsStateSaverMigration, IStateSaverMigration>();
            }
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
                if (saverOptions is IAutoMigrationOptions autoMigrationOptions
                    && autoMigrationOptions.IsAutoMigrationEnabled)
                {
                    builder.RegisterDecorator<AutoMigrationEventSaver, IEventSaver>();
                }

                builder.RegisterType<RelationalEventSaver<EventEntity>>()
                    .As<IRelationalEventSaver>()
                    .InstancePerLifetimeScope();
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
                if (loaderOptions is IAutoMigrationOptions autoMigrationOptions
                    && autoMigrationOptions.IsAutoMigrationEnabled)
                {
                    builder.RegisterDecorator<AutoMigrationEventLoader, IEventLoader>();
                }

                builder.RegisterType<RelationalEventLoader<EventEntity>>()
                    .As<IRelationalEventLoader>()
                    .InstancePerLifetimeScope();
            }
        }

        private class RelationalStateLoaderModule : Autofac.Module, IClaptrapSharedModule
        {
            private readonly IClaptrapDesign _design;

            public RelationalStateLoaderModule(IClaptrapDesign design)
            {
                _design = design;
            }

            public string Name { get; } =
                "Claptrap relational database storage provider shared components module for IStateLoader";

            public string Description { get; } =
                "Module for claptrap relational database storage provider shared components for IStateLoader";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                var loaderOptions = _design.ClaptrapStorageProviderOptions.StateLoaderOptions;
                if (loaderOptions is IAutoMigrationOptions autoMigrationOptions
                    && autoMigrationOptions.IsAutoMigrationEnabled)
                {
                    builder.RegisterDecorator<AutoMigrationStateLoader, IStateLoader>();
                }

                builder.RegisterType<RelationalStateLoader<StateEntity>>()
                    .As<IRelationalStateLoader>()
                    .InstancePerLifetimeScope();
            }
        }

        private class RelationalStateSaverModule : Autofac.Module, IClaptrapSharedModule
        {
            private readonly IClaptrapDesign _design;

            public RelationalStateSaverModule(IClaptrapDesign design)
            {
                _design = design;
            }

            public string Name { get; } =
                "Claptrap relational database storage provider shared components module for IStateSaver";

            public string Description { get; } =
                "Module for claptrap relational database storage provider shared components for IStateSaver";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                var saverOptions = _design.ClaptrapStorageProviderOptions.StateSaverOptions;
                if (saverOptions is IAutoMigrationOptions autoMigrationOptions
                    && autoMigrationOptions.IsAutoMigrationEnabled)
                {
                    builder.RegisterDecorator<AutoMigrationStateSaver, IStateSaver>();
                }

                builder.RegisterType<RelationalStateSaver<StateEntity>>()
                    .As<IRelationalStateSaver>()
                    .InstancePerLifetimeScope();
            }
        }
    }
}