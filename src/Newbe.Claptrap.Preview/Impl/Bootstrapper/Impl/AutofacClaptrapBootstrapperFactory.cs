using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Orleans;
using Newbe.Claptrap.Preview.SQLite.Module;

namespace Newbe.Claptrap.Preview
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
            var logger = _loggerFactory.CreateLogger(typeof(AutofacClaptrapBootstrapperFactory));
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AssemblyScanningModule>();
            containerBuilder.RegisterModule(new LoggingModule(_loggerFactory));
            var container = containerBuilder.Build();
            var finder = container.Resolve<IClaptrapRegistrationFinder>();

            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            logger.LogDebug("start to scan {assemblyArrayCount} assemblies, {assemblyNames}",
                assemblyArray.Length,
                assemblyArray.Select(x => x.FullName));

            logger.LogDebug("start to find claptrap");

            var claptrapRegistration = finder.Find(assemblyArray);

            logger.LogInformation("found {actorCount} actors and {handlerCount} event handlers",
                claptrapRegistration.ActorTypeRegistrations.Count(),
                claptrapRegistration.EventHandlerTypeRegistrations.Count());

            var claptrapCustomerModuleLogger = _loggerFactory.CreateLogger<ClaptrapCustomerModule>();

            var claptrapBootstrapper = new AutofacClaptrapBootstrapper(new Autofac.Module[]
            {
                new ClaptrapCustomerModule(claptrapCustomerModuleLogger, claptrapRegistration),
                new ToolsModule(),
                new ClaptrapModule(),
                new ClaptrapOrleansModule(),
                new MemoryStorageModule(),
                new SQLiteStorageModule(),
                new SerializerModule(),
            });
            return claptrapBootstrapper;
        }
    }
}