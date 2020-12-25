using Autofac;

namespace Newbe.Claptrap.Demo.Server.Services
{
    public class DemoServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<DaprActorTestService>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}