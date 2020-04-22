using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Orleans;
using Module = Autofac.Module;

namespace Newbe.Claptrap.Autofac
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
            var container = containerBuilder.Build();
            var finder = container.Resolve<IActorTypeRegistrationFinder>();

            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            logger.LogDebug("start to scan {assemblyArrayCount} assemblies, {assemblyNames}",
                assemblyArray.Length,
                assemblyArray.Select(x => x.FullName));

            logger.LogDebug("start to find actor type");
            var actorTypeRegistrations = finder
                .FindActors(assemblyArray)
                .ToArray();
            logger.LogInformation("find {count} actor types : {types}",
                actorTypeRegistrations.Length,
                actorTypeRegistrations);

            logger.LogDebug("start to find event handlers");
            var eventHandlerTypeRegistrations =
                finder
                    .FindEventHandlers(assemblyArray, actorTypeRegistrations)
                    .ToArray();
            logger.LogInformation("find {count} event handlers : {types}",
                eventHandlerTypeRegistrations.Length,
                eventHandlerTypeRegistrations);

            var claptrapCustomerModuleLogger = _loggerFactory.CreateLogger<ClaptrapCustomerModule>();

            var claptrapBootstrapper = new AutofacClaptrapBootstrapper(new Module[]
            {
                new ClaptrapCustomerModule(claptrapCustomerModuleLogger, actorTypeRegistrations,
                    eventHandlerTypeRegistrations),
                new ClaptrapModule(),
                new ClaptrapOrleansModule()
            });
            return claptrapBootstrapper;
        }
    }
}