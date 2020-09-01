using System.Diagnostics.CodeAnalysis;
using Autofac;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Modules
{
    [ExcludeFromCodeCoverage]
    public class ToolsModule : Module, IClaptrapAppModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SystemClock>()
                .As<IClock>()
                .SingleInstance();
            builder.RegisterBuildCallback(scope =>
            {
                var loggerFactory = scope.Resolve<ILoggerFactory>();
                MethodTimeLogger.LoggerFactory = loggerFactory;
            });
        }

        public string Name { get; } = "Tools and utils";
        public string Description { get; } = "Tools and utils";
    }
}