using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Design;
using Newbe.Claptrap.Preview.Abstractions.Options;
using Newbe.Claptrap.Preview.Impl.Design;
using Newbe.Claptrap.Preview.Impl.Localization;
using Newbe.Claptrap.Preview.Impl.Modules;
using Newbe.Claptrap.Preview.Orleans;
using Newbe.Claptrap.Preview.StorageProvider.SQLite.Module;
using Newtonsoft.Json;
using LK = Newbe.Claptrap.Preview.Impl.Localization.LK.L0001AutofacClaptrapBootstrapperBuilder;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class AutofacClaptrapBootstrapperBuilder : IClaptrapBootstrapperBuilder
    {
        private readonly ILogger<AutofacClaptrapBootstrapperBuilder> _logger;
        private readonly Lazy<IL> _l;

        public AutofacClaptrapBootstrapperBuilder(
            ILoggerFactory instance)
        {
            LoggerFactoryHolder.Instance = instance;
            _logger = instance.CreateLogger<AutofacClaptrapBootstrapperBuilder>();
            _l = new Lazy<IL>(CreateL);
            Options = new ClaptrapBootstrapperBuilderOptions
            {
                ScanningAssemblies = Enumerable.Empty<Assembly>(),
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
                                ActivateMinionsAtStart = false
                            },
                            EventLoadingOptions = new EventLoadingOptions
                            {
                                LoadingCountInOneBatch = 1000
                            },
                            StateRecoveryOptions = new StateRecoveryOptions
                            {
                                StateRecoveryStrategy = StateRecoveryStrategy.FromStateHolder
                            }
                        },
                        InitialStateDataFactoryType = typeof(DefaultInitialStateDataFactory),
                        StateHolderFactoryType = typeof(DeepClonerStateHolderFactory),
                        EventHandlerFactoryFactoryType = typeof(EventHandlerFactoryFactory),
                    })
                },
                ClaptrapDesignStoreProviders = new List<IClaptrapDesignStoreProvider>()
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
                _logger.LogError(e, _l.Value[LK.L001BuildException]);
                throw;
            }

            IClaptrapBootstrapper BuildCore()
            {
                var claptrapDesignStore = ScanAssembly(Options.ScanningAssemblies ?? throw new ArgumentNullException());
                var claptrapBootstrapper = new AutofacClaptrapBootstrapper(new Autofac.Module[]
                {
                    new ClaptrapCustomerModule(claptrapDesignStore),
                    new ToolsModule(),
                    new BoxModule(),
                    new ClaptrapModule(),
                    new ClaptrapOrleansModule(),
                    new MemoryStorageModule(),
                    new SQLiteStorageModule(),
                    new SerializerModule(),
                    new LocalizationModule(Options.CultureInfo),
                }, claptrapDesignStore);
                return claptrapBootstrapper;
            }
        }

        private IClaptrapDesignStore ScanAssembly(IEnumerable<Assembly> assemblies)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AssemblyScanningModule>();
            containerBuilder.RegisterModule(new LoggingModule(LoggerFactoryHolder.Instance));
            containerBuilder.RegisterModule(new LocalizationModule(Options.CultureInfo));
            var container = containerBuilder.Build();

            var factory = container.Resolve<IClaptrapDesignStoreFactory>();
            foreach (var provider in Options.ClaptrapDesignStoreProviders)
            {
                _logger.LogDebug(_l.Value[LK.L002AddProviderAsClaptrapDesignProvider], provider);
                factory.AddProvider(provider);
            }

            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            _logger.LogDebug(_l.Value[LK.L003StartToScan],
                assemblyArray.Length,
                assemblyArray.Select(x => x.FullName));
            _logger.LogDebug(_l.Value[LK.L004StartToCreateClaptrapDesign]);
            var claptrapDesignStore = factory.Create(assemblyArray);

            _logger.LogInformation(_l.Value[LK.L005ClaptrapStoreCreated]);
            _logger.LogDebug(_l.Value[LK.L006ShowAllDesign],
                JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            foreach (var configurator in Options.ClaptrapDesignStoreConfigurators)
            {
                _logger.LogDebug(_l.Value[LK.L007StartToConfigureDesignStore], configurator);
                configurator.Configure(claptrapDesignStore);
            }

            _logger.LogInformation(_l.Value[LK.L008CountDesigns],
                claptrapDesignStore.Count());
            _logger.LogDebug(_l.Value[LK.L009ShowDesignsAfterConfiguration],
                JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            _logger.LogDebug(_l.Value[LK.L010StartToValidateDesigns]);
            var validator = container.Resolve<IClaptrapDesignStoreValidator>();
            var (isOk, errorMessage) = validator.Validate(claptrapDesignStore);
            if (!isOk)
            {
                throw new ClaptrapDesignStoreValidationFailException(errorMessage);
            }

            _logger.LogInformation(_l.Value[LK.L011DesignValidationSuccess]);
            return claptrapDesignStore;
        }
    }
}