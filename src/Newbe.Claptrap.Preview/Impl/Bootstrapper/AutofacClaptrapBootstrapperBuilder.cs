using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Impl.Metadata;
using Newbe.Claptrap.Preview.Impl.Modules;
using Newbe.Claptrap.Preview.Orleans;
using Newbe.Claptrap.Preview.StorageProvider.SQLite.Module;
using Newtonsoft.Json;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class AutofacClaptrapBootstrapperBuilder : IClaptrapBootstrapperBuilder
    {
        public ILoggerFactory LoggerFactory { get; }

        private readonly List<IClaptrapDesignStoreConfigurator> _configurators;
        private readonly List<IClaptrapDesignStoreProvider> _providers;

        public AutofacClaptrapBootstrapperBuilder(
            ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            _configurators = new List<IClaptrapDesignStoreConfigurator>();
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
            _configurators.Add(configurator);
            return this;
        }

        public IClaptrapBootstrapperBuilder AddClaptrapDesignStoreProvider(IClaptrapDesignStoreProvider provider)
        {
            _providers.Add(provider);
            return this;
        }

        public IClaptrapBootstrapper Build()
        {
            var claptrapDesignStore = ScanAssembly(_assemblies ?? throw new ArgumentNullException());
            var claptrapCustomerModuleLogger = LoggerFactory.CreateLogger<ClaptrapCustomerModule>();
            var claptrapBootstrapper = new AutofacClaptrapBootstrapper(new Autofac.Module[]
            {
                new ClaptrapCustomerModule(claptrapCustomerModuleLogger, claptrapDesignStore),
                new ToolsModule(),
                new ClaptrapModule(),
                new ClaptrapOrleansModule(),
                new MemoryStorageModule(),
                new SQLiteStorageModule(),
                new SerializerModule(),
            });
            return claptrapBootstrapper;
        }

        private IClaptrapDesignStore ScanAssembly(IEnumerable<Assembly> assemblies)
        {
            var logger = LoggerFactory.CreateLogger(typeof(AutofacClaptrapBootstrapperBuilder));
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AssemblyScanningModule>();
            containerBuilder.RegisterModule(new LoggingModule(LoggerFactory));
            var container = containerBuilder.Build();
            var factory = container.Resolve<IClaptrapDesignStoreFactory>();

            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            logger.LogDebug("start to scan {assemblyArrayCount} assemblies, {assemblyNames}",
                assemblyArray.Length,
                assemblyArray.Select(x => x.FullName));

            logger.LogDebug("start to find claptrap");

            foreach (var provider in _providers)
            {
                logger.LogDebug("add {provider} as claptrap design provider", provider);
                factory.AddProvider(provider);
            }

            var claptrapDesignStore = factory.Create(assemblyArray);

            logger.LogInformation("found {actorCount} actors",
                claptrapDesignStore.Count());
            logger.LogDebug("all designs : {designs}", JsonConvert.SerializeObject(claptrapDesignStore.ToArray()));

            logger.LogDebug("start to validate all design in claptrap design store");
            var validator = container.Resolve<IClaptrapDesignStoreValidator>();
            var (isOk, errorMessage) = validator.Validate(claptrapDesignStore);
            if (!isOk)
            {
                throw new ClaptrapDesignStoreValidationFailException(errorMessage);
            }

            foreach (var claptrapDesignStoreConfigurator in _configurators)
            {
                claptrapDesignStoreConfigurator.Configurate(claptrapDesignStore);
            }

            logger.LogInformation("all design validated ok");
            return claptrapDesignStore;
        }
    }
}