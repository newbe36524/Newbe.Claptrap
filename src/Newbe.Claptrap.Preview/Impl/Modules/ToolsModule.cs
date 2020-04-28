using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace Newbe.Claptrap.Preview
{
    [ExcludeFromCodeCoverage]
    public class ToolsModule : Autofac.Module
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