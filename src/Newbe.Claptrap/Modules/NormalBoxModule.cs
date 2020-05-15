using Autofac;
using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.Modules
{
    public class NormalBoxModule : Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Normal claptrap box module";
        public string Description { get; } = "Module for registering normal claptrap box";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClaptrapBoxFactory>()
                .As<IClaptrapBoxFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<NormalClaptrapBox>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}