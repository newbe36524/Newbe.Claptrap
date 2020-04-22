using Autofac;

namespace Newbe.Claptrap.Autofac
{
    public class AssemblyScanningModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ActorTypeRegistrationFinder>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}