using Autofac;
using Newbe.Claptrap.Saga;

namespace Newbe.Claptrap.Modules
{
    public class SagaClaptrapModule : Module, IClaptrapAppModule
    {
        public string Name { get; } = "Saga";
        public string Description { get; } = "Saga Supported";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SagaClaptrap>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<SagaUserDataSerializer>()
                .As<ISagaUserDataSerializer>()
                .SingleInstance();
        }
    }
}