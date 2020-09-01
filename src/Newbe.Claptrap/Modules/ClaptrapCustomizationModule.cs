using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Modules
{
    /// <summary>
    /// Module for building Claptrap from `your code`
    /// </summary>
    public class ClaptrapCustomizationModule : Module, IClaptrapAppModule
    {
        public string Name { get; } = "Claptrap design module";
        public string Description { get; } = "Module for register types from claptrap designs";

        private readonly ILogger<ClaptrapCustomizationModule> _logger;
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public ClaptrapCustomizationModule(
            IClaptrapDesignStore claptrapDesignStore,
            ILogger<ClaptrapCustomizationModule> logger)
        {
            _logger = logger;
            _claptrapDesignStore = claptrapDesignStore;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance(_claptrapDesignStore)
                .As<IClaptrapDesignStore>()
                .SingleInstance();

            var claptrapDesigns = _claptrapDesignStore.ToArray();
            foreach (var claptrapDesign in claptrapDesigns)
            {
                var actorTypeCode = claptrapDesign.ClaptrapTypeCode;
                _logger.LogDebug("start to register actor type : {actorTypeCode}", actorTypeCode);
                foreach (var type in GetTypes(claptrapDesign).Distinct())
                {
                    builder.RegisterType(type)
                        .AsSelf()
                        .InstancePerLifetimeScope();
                }

                _logger.LogDebug("actor type registration for '{actorTypeCode}' done", actorTypeCode);
            }

            _logger.LogDebug("actor type registration done");

            _logger.LogInformation("{count} actorType have been registered into container",
                claptrapDesigns.Length);

            static IEnumerable<Type> GetTypes(IClaptrapDesign design)
            {
                yield return design.EventLoaderFactoryType;
                yield return design.EventSaverFactoryType;
                yield return design.StateHolderFactoryType;
                yield return design.StateLoaderFactoryType;
                yield return design.StateSaverFactoryType;
                yield return design.EventHandlerFactoryFactoryType;
                yield return design.InitialStateDataFactoryType;
                yield return design.EventNotifierFactoryType;
                foreach (var designEventHandlerDesign in design.EventHandlerDesigns)
                {
                    yield return designEventHandlerDesign.Value.EventHandlerType;
                }
            }
        }
    }
}