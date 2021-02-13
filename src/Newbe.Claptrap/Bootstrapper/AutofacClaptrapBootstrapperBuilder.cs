using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        private readonly ILogger<AutofacClaptrapBootstrapperBuilder> _logger;
        private readonly Lazy<IL> _l;

        public AutofacClaptrapBootstrapperBuilder(
            ILoggerFactory? loggerFactory = default)
        {
            _loggerFactory = loggerFactory ?? new NullLoggerFactory();
            LoggerFactoryHolder.Instance = _loggerFactory;
            _logger = _loggerFactory.CreateLogger<AutofacClaptrapBootstrapperBuilder>();
            _l = new Lazy<IL>(CreateL);
            Options = new ClaptrapBootstrapperBuilderOptions
            {
                DesignTypes = Enumerable.Empty<Type>(),
                ModuleTypes = Enumerable.Empty<Type>(),
                ClaptrapDesignStoreConfigurators = new List<IClaptrapDesignStoreConfigurator>(),
                ClaptrapDesignStoreProviders = new List<IClaptrapDesignStoreProvider>(),
                ClaptrapModuleProviders = new List<IClaptrapModuleProvider>()
            };
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
                using var storeScope = container.BeginLifetimeScope();
                var factory = storeScope.Resolve<IClaptrapDesignStoreFactory>();
                var store =
                    CreateDefaultStore(factory, Options.DesignTypes ?? throw new ArgumentNullException());
                // minion can save nothing
                this.ConfigureClaptrapDesign(x => x.IsMinion(),
                    x =>
                    {
                        x.EventSaverFactoryType = typeof(EmptyEventSaverFactory);
                        x.ClaptrapStorageProviderOptions.EventSaverOptions = new EmptyEventSaverOptions();
                    });

                IEnumerable<Module> appAutofacModules = Enumerable.Empty<Module>();
                var (appProviderScope, appProviders) = CreateAppProviders(container, store);
                using (appProviderScope)
                {
                    var claptrapDesignStoreConfigurators =
                        appProviders.SelectMany(x => x.GetClaptrapDesignStoreConfigurators());
                    var allConfig =
                        claptrapDesignStoreConfigurators.Concat(Options.ClaptrapDesignStoreConfigurators);

                    store = ConfigStore(store, allConfig);
                    var validator = storeScope.Resolve<IClaptrapDesignStoreValidator>();
                    store = ValidateStore(store, validator);

                    var appModules = appProviders
                        .SelectMany(x =>
                        {
                            var ms = x.GetClaptrapApplicationModules().ToArray();
                            _logger.LogDebug("Found {Count} claptrap application modules from {Type} : {Modules}",
                                ms.Length,
                                x,
                                ms.Select(a => a.Name));
                            return ms;
                        })
                        .ToArray();

                    _logger.LogInformation(
                        "Scanned {TypesCount}, and found {Count} claptrap application modules : {Modules}",
                        Options.ModuleTypes.Count(),
                        appModules.Length,
                        appModules.Select(x => x.Name));
                    appAutofacModules = appModules
                        .OfType<Module>()
                        .ToArray();
                    _logger.LogInformation(
                        "Filtered and found {Count} Autofac modules : {@Modules}",
                        appAutofacModules.Count(),
                        appAutofacModules);
                }

                var providers = ScanClaptrapModuleProviders();

                var claptrapBootstrapper =
                    new AutofacClaptrapBootstrapper(
                        providers,
                        appAutofacModules
                            .Concat(new[] {new ClaptrapBootstrapperBuilderOptionsModule(Options)}),
                        store);

                return claptrapBootstrapper;
            }
        }

        private (ILifetimeScope scope, IClaptrapAppProvider[] appProviders) CreateAppProviders(
            ILifetimeScope container,
            IClaptrapDesignStore claptrapDesignStore)
        {
            var scope = container.BeginLifetimeScope(innerBuilder =>
            {
                var providerTypes = Options.ModuleTypes
                    .Where(x => x.IsClass && !x.IsAbstract)
                    .Where(x => x.GetInterface(typeof(IClaptrapAppProvider).FullName!) != null)
                    .ToArray();
                _logger.LogDebug("Found type {ProviderTypes} as {Name}",
                    providerTypes,
                    nameof(IClaptrapAppProvider));
                innerBuilder.RegisterTypes(providerTypes)
                    .As<IClaptrapAppProvider>()
                    .InstancePerLifetimeScope();
                innerBuilder.RegisterInstance(claptrapDesignStore);
            });
            var appProviders = scope.Resolve<IEnumerable<IClaptrapAppProvider>>();
            return (scope, appProviders.ToArray());
        }

        private Type[] ScanClaptrapModuleProviders()
        {
            var providers = Options.ModuleTypes
                .Where(x => x.IsClass && !x.IsAbstract)
                .Where(x => x.GetInterface(typeof(IClaptrapModuleProvider).FullName!) != null)
                .ToArray();
            _logger.LogInformation(
                "Scanned {TypeCount}, and found {Count} claptrap modules providers : {Modules}",
                Options.ModuleTypes.Count(),
                providers.Length,
                providers);
            return providers;
        }

        private IContainer CreateContainerForScanning()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ClaptrapDesignScanningModule>();
            builder.RegisterModule(new LoggingModule(_loggerFactory));
            builder.RegisterModule(new LocalizationModule(Options.ClaptrapLocalizationOptions));
            builder.RegisterInstance(Options);
            var container = builder.Build();
            return container;
        }

        private IClaptrapDesignStore CreateDefaultStore(
            IClaptrapDesignStoreFactory factory,
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
            return claptrapDesignStore;
        }


        private IClaptrapDesignStore ConfigStore(
            IClaptrapDesignStore claptrapDesignStore,
            IEnumerable<IClaptrapDesignStoreConfigurator> configurators)
        {
            _logger.LogInformation(_l.Value[LK.claptrap_design_store_created__start_to_configure_it]);
            _logger.LogDebug(_l.Value[LK.all_designs____designs_],
                JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            foreach (var configurator in configurators)
            {
                _logger.LogDebug(_l.Value[LK.start_to_configure_claptrap_design_store_by__configurator_],
                    configurator);
                configurator.Configure(claptrapDesignStore);
            }

            _logger.LogInformation(_l.Value[LK.found__actorCount__claptrap_designs],
                claptrapDesignStore.Count());
            _logger.LogDebug(_l.Value[LK.all_designs_after_configuration___designs_],
                JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));
            return claptrapDesignStore;
        }


        private IClaptrapDesignStore ValidateStore(
            IClaptrapDesignStore claptrapDesignStore,
            IClaptrapDesignStoreValidator validator)
        {
            if (Options.ClaptrapDesignStoreValidationOptions.Enabled)
            {
                _logger.LogDebug(_l.Value[LK.start_to_validate_all_design_in_claptrap_design_store]);
                var (isOk, errorMessage) = validator.Validate(claptrapDesignStore);
                if (!isOk)
                {
                    throw new ClaptrapDesignStoreValidationFailException(errorMessage);
                }

                _logger.LogInformation(_l.Value[LK.all_design_validated_ok]);
            }
            else
            {
                _logger.LogInformation("Validation disabled");
            }

            return claptrapDesignStore;
        }
    }
}