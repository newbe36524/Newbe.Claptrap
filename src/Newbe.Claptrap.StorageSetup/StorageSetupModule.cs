using Autofac;

namespace Newbe.Claptrap.StorageSetup
{
    public class StorageSetupModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<DockerComposeService>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<DataBaseService>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}