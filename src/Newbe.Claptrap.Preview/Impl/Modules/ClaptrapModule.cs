using System.Diagnostics.CodeAnalysis;
using Autofac;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventHandler;
using Newbe.Claptrap.Preview.EventStore;
using Newbe.Claptrap.Preview.StateStore;

namespace Newbe.Claptrap.Preview
{
    [ExcludeFromCodeCoverage]
    public class ClaptrapModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DeepClonerStateHolder>()
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
            builder.RegisterType<InitialStateDataFactory>()
                .As<IInitialStateDataFactory>()
                .SingleInstance();
            builder.RegisterType<MemoryStateStore>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<EventHandlerFactory>()
                .As<IEventHandlerFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<Actor>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}