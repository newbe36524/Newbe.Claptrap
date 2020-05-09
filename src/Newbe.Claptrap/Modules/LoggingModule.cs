using Autofac;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Modules
{
    public class LoggingModule : Module
    {
        private readonly ILoggerFactory _loggerFactory;

        public LoggingModule(
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance(_loggerFactory)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();
        }
    }
}