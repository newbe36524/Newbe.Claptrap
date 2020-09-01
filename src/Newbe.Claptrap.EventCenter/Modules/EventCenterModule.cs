using Autofac;

namespace Newbe.Claptrap.EventCenter.Modules
{
    public class EventCenterModule : Module, IClaptrapAppModule
    {
        public string Name { get; } = "Claptrap EventCenter module";
        public string Description { get; } = "Module for EventCenter";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<EmptyMinionLocator>()
                .As<IMinionLocator>()
                .IfNotRegistered(typeof(IMinionLocator))
                .SingleInstance();

            builder.RegisterType<EmptyEventCenter>()
                .As<IEventCenter>()
                .IfNotRegistered(typeof(IEventCenter))
                .SingleInstance();
        }
    }
}