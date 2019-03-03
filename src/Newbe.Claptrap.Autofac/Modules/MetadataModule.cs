using Autofac;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Autofac.Modules
{
    public class MetadataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ReflectionActorMetadataProvider>()
                .As<IActorMetadataProvider>()
                .SingleInstance();
        }
    }
}