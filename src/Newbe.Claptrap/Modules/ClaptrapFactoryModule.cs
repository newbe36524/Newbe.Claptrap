using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace Newbe.Claptrap.Modules
{
    [ExcludeFromCodeCoverage]
    public class ClaptrapFactoryModule : Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Claptrap factory module";
        public string Description { get; } = "Module for registering ClaptrapFactory and some common implementations";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ClaptrapFactory>()
                .As<IClaptrapFactory>()
                .SingleInstance();

            builder.RegisterType<DesignBaseEventHandlerFactory>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}