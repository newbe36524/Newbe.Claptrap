using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Logging;

namespace Newbe.Claptrap.Preview.Impl.Modules
{
    /// <summary>
    /// Module for building Claptrap from `your code`
    /// </summary>
    public class ClaptrapCustomerModule : Module
    {
        private static readonly ILog Logger = LogProvider.For<ClaptrapCustomerModule>();
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public ClaptrapCustomerModule(
            IClaptrapDesignStore claptrapDesignStore)
        {
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
                var actorTypeCode = claptrapDesign.Identity.TypeCode;
                Logger.Debug("start to register actor type : {actorTypeCode}", actorTypeCode);
                foreach (var type in GetTypes(claptrapDesign))
                {
                    builder.RegisterType(type)
                        .AsSelf()
                        .InstancePerLifetimeScope();
                }

                Logger.Debug("actor type registration for '{actorTypeCode}' done", actorTypeCode);
            }

            Logger.Debug("actor type registration done");

            Logger.Info("{count} actorType have been registered into container",
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
                foreach (var designEventHandlerDesign in design.EventHandlerDesigns)
                {
                    yield return designEventHandlerDesign.Value.EventHandlerType;
                }
            }
        }
    }
}