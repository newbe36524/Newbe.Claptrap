using Autofac;

namespace Newbe.Claptrap.EventCenter.Dapr.Modules
{
    public class DaprEventCenterModule : Module, IClaptrapAppModule
    {
        public string Name { get; } = "Claptrap Dapr EventCenter module";
        public string Description { get; } = "Module for Claptrap EventCenter implement by Dapr Pubsub";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DaprPubsubSender>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<ClaptrapHandler>()
                .As<IClaptrapHandler>()
                .SingleInstance();

            builder.RegisterType<StartupFilter>()
                .AsImplementedInterfaces();
        }
    }
}