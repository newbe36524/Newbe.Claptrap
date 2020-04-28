using Autofac;

namespace Newbe.Claptrap.Autofac
{
    public class ToolsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SystemClock>()
                .As<IClock>()
                .SingleInstance();
        }
    }
}