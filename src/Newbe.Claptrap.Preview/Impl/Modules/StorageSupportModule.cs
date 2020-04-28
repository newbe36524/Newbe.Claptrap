using System;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Newbe.Claptrap.Preview.EventStore;
using Newbe.Claptrap.Preview.StateStore;

namespace Newbe.Claptrap.Preview
{
    [ExcludeFromCodeCoverage]
    public abstract class StorageSupportModule : Autofac.Module
    {
        public EventStoreProvider EventStoreProvider { get; }
        public StateStoreProvider StateStoreProvider { get; }
        public Type? EventStoreType { get; set; }
        public Type? EventStoreFactoryHandlerType { get; set; }

        public Type? StateStoreType { get; set; }
        public Type? StateStoreFactoryHandlerType { get; set; }

        protected StorageSupportModule(
            EventStoreProvider eventStoreProvider,
            StateStoreProvider stateStoreProvider)
        {
            EventStoreProvider = eventStoreProvider;
            StateStoreProvider = stateStoreProvider;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            RegisterEventStore(builder);
            RegisterStateStore(builder);
        }

        protected virtual void RegisterEventStore(ContainerBuilder builder)
        {
            if (EventStoreType != null && EventStoreFactoryHandlerType != null)
            {
                builder.RegisterType(EventStoreType)
                    .AsSelf()
                    .InstancePerLifetimeScope();
                builder.RegisterType(EventStoreFactoryHandlerType)
                    .Keyed<IIEventStoreFactoryHandler>(EventStoreProvider);
            }
        }

        protected virtual void RegisterStateStore(ContainerBuilder builder)
        {
            if (StateStoreType != null && StateStoreFactoryHandlerType != null)
            {
                builder.RegisterType(StateStoreType)
                    .AsSelf()
                    .InstancePerLifetimeScope();
                builder.RegisterType(StateStoreFactoryHandlerType)
                    .Keyed<IStateStoreFactoryHandler>(StateStoreProvider);
            }
        }
    }
}