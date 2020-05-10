using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class XunitLoggingModule : Autofac.Module
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public XunitLoggingModule(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var loggerFactory = new LoggerFactory(new[] {new XunitLoggerProvider(_testOutputHelper)});
            builder.RegisterInstance(loggerFactory)
                .AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();
        }
    }
}