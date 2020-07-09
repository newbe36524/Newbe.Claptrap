using System.Collections.Generic;
using Autofac;
using Module = Autofac.Module;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Modules.Providers
{
    public class ClaptrapModuleProvider : IClaptrapModuleProvider
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public ClaptrapModuleProvider(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public IEnumerable<IClaptrapMasterModule> GetClaptrapMasterClaptrapModules(IClaptrapIdentity identity)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            if (design.ClaptrapOptions.EventCenterOptions.EventCenterType == EventCenterType.RabbitMQ)
            {
                yield return new ClaptrapSharedModule();
            }
        }

        private class ClaptrapSharedModule : Module, IClaptrapMasterModule
        {
            public string Name { get; } = "Claptrap RabbitMQ EventCenter module";
            public string Description { get; } = "Module for claptrap to send event by RabbitMQ";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);

                builder.RegisterType<RabbitMQEventCenter>()
                    .As<IEventCenter>()
                    .SingleInstance();
            }
        }
    }
}