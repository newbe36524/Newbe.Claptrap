using Autofac;
using Newbe.Claptrap.EventCenter;

namespace Newbe.Claptrap.Dapr.Modules
{
    public class DaprEventCenterModule : Module, IClaptrapMasterModule
    {
        public string Name { get; } = "Dapr Rpc event center notifier";
        public string Description { get; } = "Module for support event notifier based on Dapr Rpc call";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<DaprRpcEventCenter>()
                .As<IEventCenter>()
                .SingleInstance();
        }
    }
}