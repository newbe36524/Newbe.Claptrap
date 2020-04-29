using System.Diagnostics.CodeAnalysis;
using Autofac;
using Newbe.Claptrap.Preview.Abstractions;

namespace Newbe.Claptrap.Preview.Impl.Modules
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