using System;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Newbe.Claptrap.Preview.Abstractions;

namespace Newbe.Claptrap.Preview.Impl.Modules
{
    [ExcludeFromCodeCoverage]
    public abstract class StorageSupportModule : Module
    {
        public Type? EventStoreType { get; set; }

        public Type? StateStoreType { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            RegisterEventStore(builder);
            RegisterStateStore(builder);
        }

        protected virtual void RegisterEventStore(ContainerBuilder builder)
        {
            if (EventStoreType != null)
            {
                builder.RegisterType(EventStoreType)
                    .AsSelf()
                    .InstancePerLifetimeScope();
            }
        }

        protected virtual void RegisterStateStore(ContainerBuilder builder)
        {
            if (StateStoreType != null)
            {
                builder.RegisterType(StateStoreType)
                    .AsSelf()
                    .InstancePerLifetimeScope();
            }
        }
    }
}