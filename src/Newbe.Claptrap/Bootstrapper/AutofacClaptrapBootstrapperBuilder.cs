using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Design;
using Newbe.Claptrap.Localization.Modules;
using Newbe.Claptrap.Modules;
using Newbe.Claptrap.StateHolder;
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
                DesignTypes = Enumerable.Empty<Type>(),
                ModuleTypes = Enumerable.Empty<Type>(),
                CultureInfo = CultureInfo.CurrentCulture,
                ClaptrapDesignStoreConfigurators = new List<IClaptrapDesignStoreConfigurator>(),
                ClaptrapDesignStoreProviders = new List<IClaptrapDesignStoreProvider>(),
                ClaptrapModuleProviders = new List<IClaptrapModuleProvider>(),
            };

            this
                .ConfigureClaptrapDesign(
                    x => x.ClaptrapOptions.StateSavingOptions == null,
                    x => x.ClaptrapOptions.StateSavingOptions = new StateSavingOptions
                    {
                        SavingWindowTime = TimeSpan.FromSeconds(10),
                        SaveWhenDeactivateAsync = true,
                        SavingWindowVersionLimit = 1000,
                    })
                .ConfigureClaptrapDesign(
                    x => x.ClaptrapOptions.MinionActivationOptions == null,
                    x => x.ClaptrapOptions.MinionActivationOptions = new MinionActivationOptions
                    {
                        ActivateMinionsAtMasterStart = false
                    })
                .ConfigureClaptrapDesign(
                    x => x.ClaptrapOptions.EventLoadingOptions == null,
                    x => x.ClaptrapOptions.EventLoadingOptions = new EventLoadingOptions
                    {
                        LoadingCountInOneBatch = 1000
                    })
                .ConfigureClaptrapDesign(
                    x => x.ClaptrapOptions.StateRecoveryOptions == null,
                    x => x.ClaptrapOptions.StateRecoveryOptions = new StateRecoveryOptions
                    {
                        StateRecoveryStrategy = StateRecoveryStrategy.FromStore
                    })
                .ConfigureClaptrapDesign(
                    x => x.InitialStateDataFactoryType == null,
                    x => x.InitialStateDataFactoryType = typeof(DefaultInitialStateDataFactory))
                .ConfigureClaptrapDesign(
                    x => x.StateHolderFactoryType == null,
                    x => x.StateHolderFactoryType = typeof(NoChangeStateHolderFactory))
                .ConfigureClaptrapDesign(
                    x => x.EventHandlerFactoryFactoryType == null,
                    x => x.EventHandlerFactoryFactoryType = typeof(EventHandlerFactoryFactory))
                ;
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
                var builder = new ContainerBuilder();
                builder.RegisterModule<ClaptrapDesignScanningModule>();
                builder.RegisterModule(new LoggingModule(_loggerFactory));
                builder.RegisterModule(new LocalizationModule(Options.CultureInfo));
                builder.RegisterInstance(Options);
                var container = builder.Build();
                IClaptrapDesignStore? claptrapDesignStore = null;
                using (var scope = container.BeginLifetimeScope())
                {
                    var factory = scope.Resolve<IClaptrapDesignStoreFactory>();
                    var validator = scope.Resolve<IClaptrapDesignStoreValidator>();
                    claptrapDesignStore =
                        CreateClaptrapDesignStore(
                            factory,
                            validator,
                            Options.DesignTypes ?? throw new ArgumentNullException());
                }

                IClaptrapApplicationModule[]? applicationModules = null;
                using (var scope = container.BeginLifetimeScope(innerBuilder =>
                {
                    var providerTypes = Options.ModuleTypes
                        .Where(x => x.IsClass && !x.IsAbstract)
                        .Where(x => x.GetInterface(typeof(IClaptrapApplicationModuleProvider).FullName) != null)
                        .ToArray();
                    _logger.LogDebug("Found type {providerTypes} as {name}",
                        providerTypes,
                        nameof(IClaptrapApplicationModuleProvider));
                    innerBuilder.RegisterTypes(providerTypes)
                        .As<IClaptrapApplicationModuleProvider>()
                        .InstancePerLifetimeScope();
                    innerBuilder.RegisterInstance(claptrapDesignStore);
                }))
                {
                    var moduleProviders =
                        scope.Resolve<IEnumerable<IClaptrapApplicationModuleProvider>>();
                    applicationModules = moduleProviders
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
                }

                var autofacModules = applicationModules
                    .OfType<Module>()
                    .ToArray();

                _logger.LogInformation(
                    "Filtered and found {count} Autofac modules : {modules}",
                    autofacModules.Length,
                    autofacModules);

                // TODO move
                var providers = Options.ModuleTypes
                    .Where(x => x.IsClass && !x.IsAbstract)
                    .Where(x => x.GetInterface(typeof(IClaptrapModuleProvider).FullName) != null)
                    .ToArray();
                _logger.LogInformation(
                    "Scanned {typeCount}, and found {count} claptrap modules providers : {modules}",
                    Options.ModuleTypes.Count(),
                    providers.Length,
                    providers);

                _applicationBuilder.RegisterTypes(providers)
                    .As<IClaptrapModuleProvider>();

                var claptrapBootstrapper =
                    new AutofacClaptrapBootstrapper(_applicationBuilder,
                        autofacModules
                            .Concat(new[] {new ClaptrapBootstrapperBuilderOptionsModule(Options)}),
                        claptrapDesignStore);

                return claptrapBootstrapper;
            }
        }

        private IClaptrapDesignStore CreateClaptrapDesignStore(
            IClaptrapDesignStoreFactory factory,
            IClaptrapDesignStoreValidator validator,
            IEnumerable<Type> types)
        {
            foreach (var provider in Options.ClaptrapDesignStoreProviders)
            {
                _logger.LogDebug(_l.Value[L002AddProviderAsClaptrapDesignProvider], provider);
                factory.AddProvider(provider);
            }

            var typeArray = types as Type[] ?? types.ToArray();
            _logger.LogDebug(_l.Value[L003StartToScan], typeArray.Length);
            _logger.LogDebug(_l.Value[L004StartToCreateClaptrapDesign]);
            var claptrapDesignStore = factory.Create(typeArray);

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