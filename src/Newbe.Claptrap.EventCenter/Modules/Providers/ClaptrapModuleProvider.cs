using System.Collections.Generic;
using Autofac;
using Module = Autofac.Module;

namespace Newbe.Claptrap.EventCenter.Modules.Providers
{
    public class ClaptrapModuleProvider : IClaptrapModuleProvider
    {
        public IEnumerable<IClaptrapMasterModule> GetClaptrapMasterClaptrapModules(IClaptrapIdentity identity)
        {
            yield return new ClaptrapSharedModule();
        }

        private class ClaptrapSharedModule : Module, IClaptrapMasterModule
        {
            public string Name { get; } = "Claptrap shared module";
            public string Description { get; } = "Module for claptrap and minion shared components";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                builder.RegisterType<EventCenterEventNotifier>()
                    .As<IEventNotifierHandler>()
                    .InstancePerDependency();
            }
        }
    }
}