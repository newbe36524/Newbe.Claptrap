using Autofac;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap.Autofac
{
    public class ClaptrapModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<NoChangeStateHolder>()
                .As<IStateHolder>()
                .SingleInstance();

            builder.RegisterType<ActorFactory>()
                .As<IActorFactory>()
                .SingleInstance();

            builder.RegisterType<EventStoreFactory>()
                .As<IEventStoreFactory>()
                .SingleInstance();

            builder.RegisterType<StateStoreFactory>()
                .As<IStateStoreFactory>()
                .SingleInstance();
            builder.RegisterType<StateDataTypeRegister>()
                .As<IStateDataTypeRegister>()
                .SingleInstance();
            builder.RegisterType<InitialStateDataFactory>()
                .As<IInitialStateDataFactory>()
                .SingleInstance();
            builder.RegisterType<MemoryStateStore>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<EventHandlerFactory>()
                .As<IEventHandlerFactory>()
                .InstancePerLifetimeScope();
            builder.RegisterType<EventHandlerRegister>()
                .As<IEventHandlerRegister>()
                .SingleInstance();

            builder.RegisterType<Actor>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}