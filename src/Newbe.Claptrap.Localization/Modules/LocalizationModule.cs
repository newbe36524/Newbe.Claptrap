using Autofac;
using Module = Autofac.Module;

namespace Newbe.Claptrap.Localization.Modules
{
    public class LocalizationModule : Module, IClaptrapAppModule
    {
        private readonly ClaptrapLocalizationOptions _claptrapLocalizationOptions;
        public string Name { get; } = "Localization module";
        public string Description { get; } = "Module for registering type for localization";

        public LocalizationModule(
            ClaptrapLocalizationOptions claptrapLocalizationOptions)
        {
            _claptrapLocalizationOptions = claptrapLocalizationOptions;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            if (_claptrapLocalizationOptions.EnableLocalization)
            {
                builder.RegisterType<L>()
                    .As<IL>()
                    .SingleInstance();
            }
            else
            {
                builder.RegisterType<DefaultL>()
                    .As<IL>()
                    .SingleInstance();
            }
        }
    }
}