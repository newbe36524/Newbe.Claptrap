using System;
using Autofac;

namespace Newbe.Claptrap.Tests
{
    public static class BuilderExtensions
    {
        public static void AddStaticClock(this ContainerBuilder builder,
            DateTime now)
        {
            builder.RegisterInstance(new StaticClock(now))
                .As<IClock>()
                .SingleInstance();
        }
    }
}