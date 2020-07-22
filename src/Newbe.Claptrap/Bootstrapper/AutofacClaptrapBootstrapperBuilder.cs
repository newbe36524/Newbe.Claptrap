using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Design;
using Newbe.Claptrap.Extensions;
using Newbe.Claptrap.Localization;
using Newbe.Claptrap.Localization.Modules;
using Newbe.Claptrap.Modules;
using Newtonsoft.Json;
using Module = Autofac.Module;

namespace Newbe.Claptrap.Bootstrapper
{
    public class AutofacClaptrapBootstrapperBuilder : IClaptrapBootstrapperBuilder
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ContainerBuilder _applicationBuilder;
        private readonly ILogger<AutofacClaptrapBootstrapperBuilder> _logger;
        private readonly Lazy<IL> _l;

        public AutofacClaptrapBootstrapperBuilder(
            ILoggerFactory loggerFactory,
            ContainerBuilder applicationBuilder)
        {
            _loggerFactory = loggerFactory;
            _applicationBuilder = applicationBuilder;
            LoggerFactoryHolder.Instance = loggerFactory;
            _logger = loggerFactory.CreateLogger<AutofacClaptrapBootstrapperBuilder>();
            _l = new Lazy<IL>(CreateL);
            Options = new ClaptrapBootstrapperBuilderOptions
            {
                DesignTypes = Enumerable.Empty<Type>(),
                ModuleTypes = Enumerable.Empty<Type>(),
                ClaptrapDesignStoreConfigurators = new List<IClaptrapDesignStoreConfigurator>(),
                ClaptrapDesignStoreProviders = new List<IClaptrapDesignStoreProvider>(),
                ClaptrapModuleProviders = new List<IClaptrapModuleProvider>(),
            };

            DefaultClaptrapDesignConfigurator.Configure(this);
        }

        private IL CreateL()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LocalizationModule(Options.ClaptrapLocalizationOptions));
            var logger = _loggerFactory.CreateLogger<L>();
            builder.RegisterInstance(logger)
                .SingleInstance();
            var container = builder.Build();
            var l = container.Resolve<IL>();
            return l;
        }

        public ClaptrapBootstrapperBuilderOptions Options { get; }

        public IClaptrapBootstrapper Build()
        {
            try
            {
                return BuildCore();
            }
            catch (Exception e)
            {
                _logger.LogError(e, _l.Value[LK.failed_to_build_claptrap_bootstrapper]);
                throw;
            }

            IClaptrapBootstrapper BuildCore()
            {
                var container = CreateContainerForScanning();

                IClaptrapDesignStore? claptrapDesignStore = null;
                // minion can save nothing
                this.ConfigureClaptrapDesign(x => x.IsMinion(),
                    x =>
                    {
                        x.EventSaverFactoryType = typeof(EmptyEventSaverFactory);
                        x.ClaptrapStorageProviderOptions.EventSaverOptions = new EmptyEventSaverOptions();
                    });
                claptrapDesignStore = CreateClaptrapDesignStore(container);

                IClaptrapApplicationModule[]? applicationModules = null;
                applicationModules = ScanApplicationModules(container, claptrapDesignStore);

                var autofacModules = applicationModules
                    .OfType<Module>()
                    .ToArray();

                _logger.LogInformation(
                    "Filtered and found {count} Autofac modules : {modules}",
                    autofacModules.Length,
                    autofacModules);

                var providers = ScanClaptrapModuleProviders();

                _applicationBuilder.RegisterTypes(providers)
                    .As<IClaptrapModuleProvider>();


                var claptrapBootstrapper =
                    new AutofacClaptrapBootstrapper(_applicationBuilder,
                        autofacModules
                            .Concat(new[] {new ClaptrapBootstrapperBuilderOptionsModule(Options)}),
                        claptrapDesignStore);

                return claptrapBootstrapper;
            }

            IContainer CreateContainerForScanning()
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule<ClaptrapDesignScanningModule>();
                builder.RegisterModule(new LoggingModule(_loggerFactory));
                builder.RegisterModule(new LocalizationModule(Options.ClaptrapLocalizationOptions));
                builder.RegisterInstance(Options);
                var container = builder.Build();
                return container;
            }

            IClaptrapDesignStore CreateClaptrapDesignStore(ILifetimeScope container)
            {
                using var scope = container.BeginLifetimeScope();
                var factory = scope.Resolve<IClaptrapDesignStoreFactory>();
                var validator = scope.Resolve<IClaptrapDesignStoreValidator>();
                var claptrapDesignStore = this.CreateClaptrapDesignStore(
                    factory,
                    validator,
                    Options.DesignTypes ?? throw new ArgumentNullException());

                return claptrapDesignStore;
            }

            IClaptrapApplicationModule[] ScanApplicationModules(ILifetimeScope container,
                IClaptrapDesignStore claptrapDesignStore)
            {
                using var scope = container.BeginLifetimeScope(innerBuilder =>
                {
                    var providerTypes = Options.ModuleTypes
                        .Where(x => x.IsClass && !x.IsAbstract)
                        .Where(x => x.GetInterface(typeof(IClaptrapApplicationModulesProvider).FullName) != null)
                        .ToArray();
                    _logger.LogDebug("Found type {providerTypes} as {name}",
                        providerTypes,
                        nameof(IClaptrapApplicationModulesProvider));
                    innerBuilder.RegisterTypes(providerTypes)
                        .As<IClaptrapApplicationModulesProvider>()
                        .InstancePerLifetimeScope();
                    innerBuilder.RegisterInstance(claptrapDesignStore);
                });
                var moduleProviders =
                    scope.Resolve<IEnumerable<IClaptrapApplicationModulesProvider>>();
                var applicationModules = moduleProviders
                    .SelectMany(x =>
                    {
                        var ms = x.GetClaptrapApplicationModules().ToArray();
                        _logger.LogDebug("Found {count} claptrap application modules from {type} : {modules}",
                            ms.Length,
                            x,
                            ms.Select(a => a.Name));
                        return ms;
                    })
                    .ToArray();

                _logger.LogInformation(
                    "Scanned {typesCount}, and found {count} claptrap application modules : {modules}",
                    Options.ModuleTypes.Count(),
                    applicationModules.Length,
                    applicationModules.Select(x => x.Name));

                return applicationModules;
            }

            Type[] ScanClaptrapModuleProviders()
            {
                var providers = Options.ModuleTypes
                    .Where(x => x.IsClass && !x.IsAbstract)
                    .Where(x => x.GetInterface(typeof(IClaptrapModuleProvider).FullName) != null)
                    .ToArray();
                _logger.LogInformation(
                    "Scanned {typeCount}, and found {count} claptrap modules providers : {modules}",
                    Options.ModuleTypes.Count(),
                    providers.Length,
                    providers);
                return providers;
            }
        }

        private IClaptrapDesignStore CreateClaptrapDesignStore(
            IClaptrapDesignStoreFactory factory,
            IClaptrapDesignStoreValidator validator,
            IEnumerable<Type> types)
        {
            foreach (var provider in Options.ClaptrapDesignStoreProviders)
            {
                _logger.LogDebug(_l.Value[LK.add__provider__as_claptrap_design_provider], provider);
                factory.AddProvider(provider);
            }

            var typeArray = types as Type[] ?? types.ToArray();
            _logger.LogDebug(_l.Value[LK.start_to_scan__assemblyArrayCount__types], typeArray.Length);
            _logger.LogDebug(_l.Value[LK.start_to_create_claptrap_design]);
            var claptrapDesignStore = factory.Create(typeArray);

            _logger.LogInformation(_l.Value[LK.claptrap_design_store_created__start_to_configure_it]);
            _logger.LogDebug(_l.Value[LK.all_designs____designs_],
                JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            foreach (var configurator in Options.ClaptrapDesignStoreConfigurators)
            {
                _logger.LogDebug(_l.Value[LK.start_to_configure_claptrap_design_store_by__configurator_], configurator);
                configurator.Configure(claptrapDesignStore);
            }

            _logger.LogInformation(_l.Value[LK.found__actorCount__claptrap_designs],
                claptrapDesignStore.Count());
            _logger.LogDebug(_l.Value[LK.all_designs_after_configuration___designs_],
                JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            _logger.LogDebug(_l.Value[LK.start_to_validate_all_design_in_claptrap_design_store]);
            var (isOk, errorMessage) = validator.Validate(claptrapDesignStore);
            if (!isOk)
            {
                throw new ClaptrapDesignStoreValidationFailException(errorMessage);
            }

            _logger.LogInformation(_l.Value[LK.all_design_validated_ok]);
            return claptrapDesignStore;
        }
    }
}