using Autofac;

namespace Newbe.Claptrap.DevTools
{
    public class ToolsServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ToolService>()
                .As<IToolService>()
                .SingleInstance();
            builder.RegisterType<LocalizationFileFactory>()
                .As<ILocalizationFileFactory>()
                .SingleInstance();
        }
    }
}