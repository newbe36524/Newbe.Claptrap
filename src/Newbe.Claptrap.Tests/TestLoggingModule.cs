using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Tests
{
    public class TestLoggingModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var provider = new ServiceCollection().AddLogging(logging => logging.AddConsole()).BuildServiceProvider();
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            builder.RegisterInstance(loggerFactory)
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();
        }
    }
}