using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace Newbe.Claptrap.Modules
{
    [ExcludeFromCodeCoverage]
    public class ToolsModule : Module, IClaptrapApplicationModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SystemClock>()
                .As<IClock>()
                .SingleInstance();
        }

        public string Name { get; } = "Tools and utils";
        public string Description { get; } = "Tools and utils";
    }
}