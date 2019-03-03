using Autofac;
using Newbe.Claptrap.Autofac.Modules;
using Newbe.Claptrap.EventHandlers;

namespace Newbe.Claptrap.Autofac
{
    public class ClaptrapModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule<ActorFactoryModule>();
            builder.RegisterModule<ActorContextModule>();
            builder.RegisterModule<EventStoreModule>();
            builder.RegisterModule<StateStoreModule>();
            builder.RegisterModule<EventHandlerModule>();
            builder.RegisterModule<StateInitializerModule>();
        }
    }
}