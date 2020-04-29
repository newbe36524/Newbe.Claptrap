using System.Diagnostics.CodeAnalysis;
using Autofac;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Impl.MemoryStore;

namespace Newbe.Claptrap.Preview.Impl.Modules
{
    [ExcludeFromCodeCoverage]
    public class ClaptrapModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DeepClonerStateHolder>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<ClaptrapFactory>()
                .As<IClaptrapFactory>()
                .SingleInstance();

            builder.RegisterType<MemoryEventStore>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<MemoryStateStore>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<DesignBaseEventHandlerFactory>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<ClaptrapActor>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}