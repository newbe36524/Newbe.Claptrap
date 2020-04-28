using System;
using Autofac;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap.Autofac
{
    public abstract class StorageSupportModule : Module
    {
        public EventStoreProvider EventStoreProvider { get; }
        public Type? EventStoreType { get; set; }
        public Type? EventStoreFactoryHandlerType { get; set; }

        protected StorageSupportModule(
            EventStoreProvider eventStoreProvider)
        {
            EventStoreProvider = eventStoreProvider;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            RegisterEventStore(builder);
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
    }
}