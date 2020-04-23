using Autofac;

namespace Newbe.Claptrap.Autofac
{
    public class AssemblyScanningModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClaptrapRegistrationFinder>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<AttributeBaseActorTypeRegistrationProvider>()
                .AsSelf()
                .As<IActorTypeRegistrationProvider>()
                .InstancePerLifetimeScope();

            builder.RegisterType<OrderBasedActorTypeRegistrationCombiner>()
                .As<IActorTypeRegistrationCombiner>()
                .SingleInstance();
        }
    }
}