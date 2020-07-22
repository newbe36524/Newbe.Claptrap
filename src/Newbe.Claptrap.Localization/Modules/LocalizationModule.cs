using Autofac;
using Module = Autofac.Module;

namespace Newbe.Claptrap.Localization.Modules
{
    public class LocalizationModule : Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Localization module";
        public string Description { get; } = "Module for registering type for localization";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<L>()
                .As<IL>()
                .SingleInstance();
        }
    }
}