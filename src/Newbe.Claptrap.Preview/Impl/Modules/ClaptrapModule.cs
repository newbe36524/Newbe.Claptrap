using System.Diagnostics.CodeAnalysis;
using Autofac;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Impl.Box;
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

            builder.RegisterType<DesignBaseEventHandlerFactory>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}