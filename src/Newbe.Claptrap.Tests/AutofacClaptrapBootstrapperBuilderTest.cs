using Autofac;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Xunit;
using Newbe.Claptrap.Bootstrapper;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class AutofacClaptrapBootstrapperBuilderTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AutofacClaptrapBootstrapperBuilderTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void NothingAdded()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            var containerBuilder = new ContainerBuilder();
            var builder = new AutofacClaptrapBootstrapperBuilder(
                new LoggerFactory(new ILoggerProvider[] {new XunitLoggerProvider(_testOutputHelper),}),
                containerBuilder);
            var claptrapBootstrapper = builder.Build();
            claptrapBootstrapper.Should().NotBeNull();
        }
    }
}