using Autofac;

namespace Newbe.Claptrap.DevTools
{
    public class ToolsServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ToolService>()
                .As<IToolService>()
                .SingleInstance();
        }
    }
}