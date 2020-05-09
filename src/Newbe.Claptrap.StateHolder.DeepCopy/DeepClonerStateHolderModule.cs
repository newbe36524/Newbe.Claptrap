using Autofac;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap
{
    public class DeepClonerStateHolderModule : Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "DeepCloner state holder module";
        public string Description { get; } = "Module for registering state holder which implement by DeepCloner";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<DeepClonerStateHolder>()
                .AsSelf()
                .SingleInstance();
        }
    }
}