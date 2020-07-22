using Autofac;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using NLog.Extensions.Logging;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class AutofacClaptrapBootstrapperBuilderTest
    {
        [Test]
        public void NothingAdded()
        {
            var serviceCollection = new ServiceCollection().AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddNLog();
            });
            var buildServiceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = buildServiceProvider.GetRequiredService<ILoggerFactory>();
            using var mocker = AutoMockHelper.Create();
            var containerBuilder = new ContainerBuilder();
            var builder = new AutofacClaptrapBootstrapperBuilder(loggerFactory, containerBuilder);
            var claptrapBootstrapper = builder.Build();
            claptrapBootstrapper.Should().NotBeNull();
        }
    }
}