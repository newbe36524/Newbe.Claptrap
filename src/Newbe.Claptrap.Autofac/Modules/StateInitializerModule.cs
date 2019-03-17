using Autofac;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac.Modules
{
    public class StateInitializerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<StateInitializerFactory>()
                .As<IStateInitializerFactory>();
            builder.RegisterType<EventSourcingStateInitializer>()
                .AsSelf();
            builder.Register(context =>
                    context.Resolve<IStateInitializerFactory>().Create(context.Resolve<IActorIdentity>()))
                .As<IStateInitializer>();
            builder.RegisterType<AutofacStateDataFactory>()
                .As<IStateDataFactory>();
        }
    }
}