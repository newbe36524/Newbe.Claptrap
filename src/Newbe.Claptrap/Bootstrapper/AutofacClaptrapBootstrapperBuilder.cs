using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Design;
using Newbe.Claptrap.Modules;
using Newbe.Claptrap.Options;
using Newtonsoft.Json;
using static Newbe.Claptrap.LK.L0001AutofacClaptrapBootstrapperBuilder;
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
                DesignAssemblies = Enumerable.Empty<Assembly>(),
                CultureInfo = CultureInfo.CurrentCulture,
                ClaptrapDesignStoreConfigurators = new List<IClaptrapDesignStoreConfigurator>
                {
                    new GlobalClaptrapDesignStoreConfigurator(new GlobalClaptrapDesign
                    {
                        ClaptrapOptions = new ClaptrapOptions
                        {
                            StateSavingOptions = new StateSavingOptions
                            {
                                SavingWindowTime = TimeSpan.FromSeconds(10),
                                SaveWhenDeactivateAsync = true,
                                SavingWindowVersionLimit = 1000,
                            },
                            MinionOptions = new MinionOptions
                            {
                                ActivateMinionsAtStart = true
                            },
                            EventLoadingOptions = new EventLoadingOptions
                            {
                                LoadingCountInOneBatch = 1000
                            },
                            StateRecoveryOptions = new StateRecoveryOptions
                            {
                                StateRecoveryStrategy = StateRecoveryStrategy.FromStore
                            }
                        },
                        InitialStateDataFactoryType = typeof(DefaultInitialStateDataFactory),
                        StateHolderFactoryType = typeof(NoChangeStateHolderFactory),
                        EventHandlerFactoryFactoryType = typeof(EventHandlerFactoryFactory),
                    })
                },
                ClaptrapDesignStoreProviders = new List<IClaptrapDesignStoreProvider>(),
                ClaptrapModuleProviders = new List<IClaptrapModuleProvider>()
            };
        }

        private IL CreateL()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LocalizationModule(Options.CultureInfo));
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
                _logger.LogError(e, _l.Value[L001BuildException]);
                throw;
            }

            IClaptrapBootstrapper BuildCore()
            {
                var claptrapDesignStore =
                    CreateClaptrapDesignStore(Options.DesignAssemblies ?? throw new ArgumentNullException());

                var assemblies = Options.ModuleAssemblies;
                var applicationModules = FindApplicationModules(assemblies, claptrapDesignStore)
                    .ToArray();
                _logger.LogInformation(
                    "Scanned {assemblies}, and found {count} claptrap application modules : {modules}",
                    assemblies,
                    applicationModules.Length,
                    applicationModules.Select(x => x.Name));

                var autofacModules = applicationModules
                    .OfType<Module>()
                    .ToArray();

                _logger.LogInformation(
                    "Filtered and found {count} Autofac modules : {modules}",
                    autofacModules.Length,
                    autofacModules);

                // TODO move
                var providers = FindClaptrapModuleProviders(assemblies).ToArray();
                _logger.LogInformation(
                    "Scanned {assemblies}, and found {count} claptrap modules providers : {modules}",
                    assemblies,
                    providers.Length,
                    providers);

                _applicationBuilder.RegisterTypes(providers.ToArray())
                    .As<IClaptrapModuleProvider>();

                var claptrapBootstrapper =
                    new AutofacClaptrapBootstrapper(_applicationBuilder,
                        autofacModules,
                        claptrapDesignStore);

                return claptrapBootstrapper;
            }
        }

        private IEnumerable<IClaptrapApplicationModule> FindApplicationModules(
            IEnumerable<Assembly> assemblies,
            IClaptrapDesignStore claptrapDesignStore)
        {
            var builder = new ContainerBuilder();
            var providerTypes = assemblies.SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract)
                .Where(x => x.GetInterface(typeof(IClaptrapApplicationModuleProvider).FullName) != null)
                .ToArray();
            _logger.LogDebug("Found type {providerTypes} as {name}",
                providerTypes,
                nameof(IClaptrapApplicationModuleProvider));
            builder.RegisterTypes(providerTypes)
                .As<IClaptrapApplicationModuleProvider>()
                .InstancePerLifetimeScope();

            builder.RegisterModule(new LoggingModule(_loggerFactory));
            builder.RegisterModule(new LocalizationModule(Options.CultureInfo));
            builder.RegisterInstance(Options);
            builder.RegisterInstance(claptrapDesignStore);

            var container = builder.Build();
            var claptrapApplicationModuleProviders =
                container.Resolve<IEnumerable<IClaptrapApplicationModuleProvider>>();
            var modules = claptrapApplicationModuleProviders.SelectMany(x =>
                {
                    var ms = x.GetClaptrapApplicationModules().ToArray();
                    _logger.LogDebug("Found {count} claptrap application modules from {type} : {modules}",
                        ms.Length,
                        x,
                        ms.Select(a => a.Name));
                    return ms;
                })
                .ToArray();
            return modules;
        }

        private static IEnumerable<Type> FindClaptrapModuleProviders(
            IEnumerable<Assembly> assemblies)
        {
            var providerTypes = assemblies.SelectMany(x => x.DefinedTypes)
                .Where(x => x.IsClass && !x.IsAbstract)
                .Where(x => x.GetInterface(typeof(IClaptrapModuleProvider).FullName) != null)
                .ToArray();
            return providerTypes;
        }

        private IClaptrapDesignStore CreateClaptrapDesignStore(IEnumerable<Assembly> assemblies)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ClaptrapDesignScanningModule>();
            builder.RegisterModule(new LoggingModule(_loggerFactory));
            builder.RegisterModule(new LocalizationModule(Options.CultureInfo));
            var container = builder.Build();

            var factory = container.Resolve<IClaptrapDesignStoreFactory>();
            foreach (var provider in Options.ClaptrapDesignStoreProviders)
            {
                _logger.LogDebug(_l.Value[L002AddProviderAsClaptrapDesignProvider], provider);
                factory.AddProvider(provider);
            }

            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            _logger.LogDebug(_l.Value[L003StartToScan],
                assemblyArray.Length,
                assemblyArray.Select(x => x.FullName));
            _logger.LogDebug(_l.Value[L004StartToCreateClaptrapDesign]);
            var claptrapDesignStore = factory.Create(assemblyArray);

            _logger.LogInformation(_l.Value[L005ClaptrapStoreCreated]);
            _logger.LogDebug(_l.Value[L006ShowAllDesign],
                JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            foreach (var configurator in Options.ClaptrapDesignStoreConfigurators)
            {
                _logger.LogDebug(_l.Value[L007StartToConfigureDesignStore], configurator);
                configurator.Configure(claptrapDesignStore);
            }

            _logger.LogInformation(_l.Value[L008CountDesigns],
                claptrapDesignStore.Count());
            _logger.LogDebug(_l.Value[L009ShowDesignsAfterConfiguration],
                JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            _logger.LogDebug(_l.Value[L010StartToValidateDesigns]);
            var validator = container.Resolve<IClaptrapDesignStoreValidator>();
            var (isOk, errorMessage) = validator.Validate(claptrapDesignStore);
            if (!isOk)
            {
                throw new ClaptrapDesignStoreValidationFailException(errorMessage);
            }

            _logger.LogInformation(_l.Value[L011DesignValidationSuccess]);
            return claptrapDesignStore;
        }
    }
}