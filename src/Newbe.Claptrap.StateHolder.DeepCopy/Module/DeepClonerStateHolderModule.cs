using Autofac;

namespace Newbe.Claptrap.Module
{
    public class DeepClonerStateHolderModule : Autofac.Module, IClaptrapAppModule
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