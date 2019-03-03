using Autofac;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap.Autofac.Modules
{
    public class EventStoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<EventStoreFactory>()
                .As<IEventStoreFactory>();
            builder.Register(context =>
                    context.Resolve<IEventStoreFactory>().Create(context.Resolve<IActorIdentity>()))
                .As<IEventStore>()
                .PerActorScope();
        }
    }
}