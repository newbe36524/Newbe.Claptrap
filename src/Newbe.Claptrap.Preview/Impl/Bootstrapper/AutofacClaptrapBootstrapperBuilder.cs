using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Impl.Metadata;
using Newbe.Claptrap.Preview.Impl.Modules;
using Newbe.Claptrap.Preview.Logging;
using Newbe.Claptrap.Preview.Orleans;
using Newbe.Claptrap.Preview.StorageProvider.SQLite.Module;
using Newtonsoft.Json;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class AutofacClaptrapBootstrapperBuilder : IClaptrapBootstrapperBuilder
    {
        private static readonly ILog Logger = LogProvider.For<AutofacClaptrapBootstrapperBuilder>();
        private readonly List<IClaptrapDesignStoreConfigurator> _configurators;
        private readonly List<IClaptrapDesignStoreProvider> _providers;

        public AutofacClaptrapBootstrapperBuilder()
        {
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
                Logger.ErrorException("failed to build {name}", e, nameof(IClaptrapBootstrapperBuilder));
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
                }, claptrapDesignStore);
                return claptrapBootstrapper;
            }
        }

        private IClaptrapDesignStore ScanAssembly(IEnumerable<Assembly> assemblies)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AssemblyScanningModule>();
            var container = containerBuilder.Build();

            var factory = container.Resolve<IClaptrapDesignStoreFactory>();
            foreach (var provider in _providers)
            {
                Logger.Debug("add {provider} as claptrap design provider", provider);
                factory.AddProvider(provider);
            }

            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            Logger.Debug("start to scan {assemblyArrayCount} assemblies, {assemblyNames}",
                assemblyArray.Length,
                assemblyArray.Select(x => x.FullName));
            Logger.Debug("start to find claptrap");
            var claptrapDesignStore = factory.Create(assemblyArray);

            Logger.Info("claptrap design store create, start to configure it");
            Logger.Debug("all designs : {designs}", JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            foreach (var configurator in _configurators)
            {
                Logger.Debug("start to configure claptrap design store by {configurator}", configurator);
                configurator.Configure(claptrapDesignStore);
            }

            Logger.Info("found {actorCount} actors",
                claptrapDesignStore.Count());
            Logger.Debug("all designs after configuration: {designs}",
                JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            Logger.Debug("start to validate all design in claptrap design store");
            var validator = container.Resolve<IClaptrapDesignStoreValidator>();
            var (isOk, errorMessage) = validator.Validate(claptrapDesignStore);
            if (!isOk)
            {
                throw new ClaptrapDesignStoreValidationFailException(errorMessage);
            }

            Logger.Info("all design validated ok");
            return claptrapDesignStore;
        }
    }
}