using Autofac;

namespace Newbe.Claptrap.Orleans.Modules
{
    public class OrleansDirectlyEventCenterModule : Module, IClaptrapMasterModule
    {
        public string Name { get; } = "Orleans directly event center notifier";
        public string Description { get; } = "Module for support event notifier based on Orleans directly call";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<MemoryOrleansEventCenter>()
                .As<IEventCenter>()
                .SingleInstance();
        }
    }
}