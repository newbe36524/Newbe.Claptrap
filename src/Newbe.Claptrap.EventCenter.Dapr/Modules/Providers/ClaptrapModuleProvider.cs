using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.EventCenter.Dapr;
using Module = Autofac.Module;

namespace Newbe.Claptrap.EventCenter.Dapr.Modules.Providers
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
            if (design.ClaptrapOptions.EventCenterOptions.EventCenterType == EventCenterType.DaprPubsub)
            {
                yield return new ClaptrapSharedModule();
            }
        }

        private class ClaptrapSharedModule : Module, IClaptrapMasterModule
        {
            public string Name { get; } = "Claptrap DaprPubsub EventCenter module";
            public string Description { get; } = "Module for claptrap to send event by DaprPubsub";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);

                builder.RegisterType<DaprEventCenter>()
                    .As<IEventCenter>()
                    .SingleInstance();
            }
        }
    }
}