using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Impl.Localization;
using Newbe.Claptrap.Preview.Impl.Metadata;
using Newbe.Claptrap.Preview.Impl.Modules;
using Newbe.Claptrap.Preview.Logging;
using Newbe.Claptrap.Preview.Orleans;
using Newbe.Claptrap.Preview.StorageProvider.SQLite.Module;
using Newtonsoft.Json;
using LK = Newbe.Claptrap.Preview.Impl.Localization.LK.L0001AutofacClaptrapBootstrapperBuilder;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class AutofacClaptrapBootstrapperBuilder : IClaptrapBootstrapperBuilder
    {
        private readonly IL _l;
        private static readonly ILog Logger = LogProvider.For<AutofacClaptrapBootstrapperBuilder>();
        private readonly List<IClaptrapDesignStoreConfigurator> _configurators;
        private readonly List<IClaptrapDesignStoreProvider> _providers;

        public AutofacClaptrapBootstrapperBuilder(
            IL l = default)
        {
            _l = l ?? L.Instance;
            _configurators = new List<IClaptrapDesignStoreConfigurator>
            {
                new GlobalClaptrapDesignStoreConfigurator(new GlobalClaptrapDesign
                {
                    StateOptions = new StateOptions
                    {
                        SavingWindowTime = TimeSpan.FromSeconds(10),
                        SaveWhenDeactivateAsync = true,
                        SavingWindowVersionLimit = 1000
                    },
                    InitialStateDataFactoryType = typeof(DefaultInitialStateDataFactory),
                    StateHolderFactoryType = typeof(DeepClonerStateHolderFactory)
                })
            };
            _providers = new List<IClaptrapDesignStoreProvider>();
        }

        private IEnumerable<Assembly>? _assemblies;

        public IClaptrapBootstrapperBuilder AddAssemblies(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
            return this;
        }

        public IClaptrapBootstrapperBuilder AddClaptrapDesignStoreConfigurator(
            IClaptrapDesignStoreConfigurator configurator)
        {
            _configurators.Insert(0, configurator);
            return this;
        }

        public IClaptrapBootstrapperBuilder AddClaptrapDesignStoreProvider(IClaptrapDesignStoreProvider provider)
        {
            _providers.Add(provider);
            return this;
        }

        public IClaptrapBootstrapper Build()
        {
            try
            {
                return BuildCore();
            }
            catch (Exception e)
            {
                Logger.ErrorException(_l[LK.L001BuildException], e);
                throw;
            }

            IClaptrapBootstrapper BuildCore()
            {
                var claptrapDesignStore = ScanAssembly(_assemblies ?? throw new ArgumentNullException());
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
                    new LocalizationModule(),
                }, claptrapDesignStore);
                return claptrapBootstrapper;
            }
        }

        private IClaptrapDesignStore ScanAssembly(IEnumerable<Assembly> assemblies)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AssemblyScanningModule>();
            containerBuilder.RegisterModule<LocalizationModule>();
            var container = containerBuilder.Build();

            var factory = container.Resolve<IClaptrapDesignStoreFactory>();
            foreach (var provider in _providers)
            {
                Logger.Debug(_l[LK.L002AddProviderAsClaptrapDesignProvider], provider);
                factory.AddProvider(provider);
            }

            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            Logger.Debug(_l[LK.L003StartToScan],
                assemblyArray.Length,
                assemblyArray.Select(x => x.FullName));
            Logger.Debug(_l[LK.L004StartToCreateClaptrapDesign]);
            var claptrapDesignStore = factory.Create(assemblyArray);

            Logger.Info(_l[LK.L005ClaptrapStoreCreated]);
            Logger.Debug(_l[LK.L006ShowAllDesign], JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            foreach (var configurator in _configurators)
            {
                Logger.Debug(_l[LK.L007StartToConfigureDesignStore], configurator);
                configurator.Configure(claptrapDesignStore);
            }

            Logger.Info(_l[LK.L008CountDesigns],
                claptrapDesignStore.Count());
            Logger.Debug(_l[LK.L009ShowDesignsAfterConfiguration],
                JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            Logger.Debug(_l[LK.L010StartToValidateDesigns]);
            var validator = container.Resolve<IClaptrapDesignStoreValidator>();
            var (isOk, errorMessage) = validator.Validate(claptrapDesignStore);
            if (!isOk)
            {
                throw new ClaptrapDesignStoreValidationFailException(errorMessage);
            }

            Logger.Info(_l[LK.L011DesignValidationSuccess]);
            return claptrapDesignStore;
        }
    }
}