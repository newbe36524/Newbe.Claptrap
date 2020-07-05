using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Module = Autofac.Module;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Modules.Providers
{
    public class ClaptrapModuleProvider : IClaptrapModuleProvider
    {
        private readonly ILogger<ClaptrapModuleProvider> _logger;
        private readonly IOptions<ClaptrapServerOptions> _options;

        public ClaptrapModuleProvider(
            ILogger<ClaptrapModuleProvider> logger,
            IOptions<ClaptrapServerOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        public IEnumerable<IClaptrapMasterModule> GetClaptrapMasterClaptrapModules(IClaptrapIdentity identity)
        {
            var enabled = _options.Value.RabbitMQ?.Enabled;
            _logger.LogTrace("RabbitMQ Enabled : {value}", enabled);
            if (enabled == true)
            {
                yield return new ClaptrapSharedModule();
            }
        }

        private class ClaptrapSharedModule : Module, IClaptrapMasterModule
        {
            public string Name { get; } = "Claptrap shared module";
            public string Description { get; } = "Module for claptrap and minion shared components";

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);

                builder.RegisterType<RabbitMQEventCenter>()
                    .As<IEventCenter>()
                    .SingleInstance();
            }
        }
    }
}