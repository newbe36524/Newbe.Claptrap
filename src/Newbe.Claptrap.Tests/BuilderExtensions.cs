using System;
using Autofac;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public static class BuilderExtensions
    {
        public static void AddLogging(this ContainerBuilder builder,
            ITestOutputHelper testOutputHelper)
        {
            builder.RegisterModule(new XunitLoggingModule(testOutputHelper));
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