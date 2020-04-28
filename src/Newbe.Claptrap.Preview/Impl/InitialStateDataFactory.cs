using System;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.StateStore;

namespace Newbe.Claptrap.Preview
{
    public class InitialStateDataFactory : IInitialStateDataFactory
    {
        private readonly IIndex<string, IInitialStateDataFactoryHandler> _handlers;
        private readonly ILogger<InitialStateDataFactory> _logger;

        public InitialStateDataFactory(
            IIndex<string, IInitialStateDataFactoryHandler> handlers,
            ILogger<InitialStateDataFactory> logger)
        {
            _handlers = handlers;
            _logger = logger;
        }

        public Task<IStateData> Create(IActorIdentity identity)
        {
            if (!_handlers.TryGetValue(identity.TypeCode, out var handler))
            {
                // TODO 
                throw new Exception($"missing handler for type code : {identity.TypeCode}");
            }

            _logger.LogInformation("custom handler for creating state data found {actorTypeCode} {handler}",
                identity.TypeCode,
                handler);
            return handler.Create(identity);
        }
    }
}