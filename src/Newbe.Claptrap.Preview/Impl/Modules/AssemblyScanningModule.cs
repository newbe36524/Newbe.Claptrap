using Autofac;

namespace Newbe.Claptrap.Preview
{
    /// <summary>
    /// Module for building <see cref="IClaptrapRegistrationFinder"/>
    /// </summary>
    public class AssemblyScanningModule : Autofac.Module
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