using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public static class BuilderExtensions
    {
        public static void AddLogging(this ContainerBuilder builder,
            ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory(new[] {new XunitLoggerProvider(testOutputHelper)});
            builder.RegisterInstance(loggerFactory)
                .AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();
        }

        public static void AddStaticClock(this ContainerBuilder builder,
            DateTime now)
        {
            builder.RegisterInstance(new StaticClock(now))
                .As<IClock>()
                .SingleInstance();
        }
    }
}