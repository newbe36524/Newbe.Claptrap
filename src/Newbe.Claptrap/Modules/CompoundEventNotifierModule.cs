using Autofac;
using Newbe.Claptrap.EventNotifier;

namespace Newbe.Claptrap.Modules
{
    public class CompoundEventNotifierModule : Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Compound Event Notifier Module";

        public string Description { get; } =
            "Module for registering Compound Event Notifier. It will notify event to all event notifier handler concurrently";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<CompoundEventNotifier>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}