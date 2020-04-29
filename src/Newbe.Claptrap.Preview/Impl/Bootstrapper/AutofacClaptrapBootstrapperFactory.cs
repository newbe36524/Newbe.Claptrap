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
    public class AutofacClaptrapBootstrapperFactory : IClaptrapBootstrapperFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public AutofacClaptrapBootstrapperFactory(
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IClaptrapBootstrapper Create(IEnumerable<Assembly> assemblies)
        {
            var claptrapDesignStore = ScanAssembly(assemblies);
            var claptrapCustomerModuleLogger = _loggerFactory.CreateLogger<ClaptrapCustomerModule>();
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
            var logger = _loggerFactory.CreateLogger(typeof(AutofacClaptrapBootstrapperFactory));
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AssemblyScanningModule>();
            containerBuilder.RegisterModule(new LoggingModule(_loggerFactory));
            var container = containerBuilder.Build();
            var claptrapDesignStoreFactory = container.Resolve<IClaptrapDesignStoreFactory>();

            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            logger.LogDebug("start to scan {assemblyArrayCount} assemblies, {assemblyNames}",
                assemblyArray.Length,
                assemblyArray.Select(x => x.FullName));

            logger.LogDebug("start to find claptrap");

            var claptrapDesignStore = claptrapDesignStoreFactory.Create(assemblyArray);

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

            logger.LogInformation("all design validated ok");
            return claptrapDesignStore;
        }
    }
}