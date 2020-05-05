using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class EmptyEventHandler : IEventHandler
    {
        private readonly ILogger<EmptyEventHandler> _logger;

        public EmptyEventHandler(
            ILogger<EmptyEventHandler> logger)
        {
            _logger = logger;
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public Task<IState> HandleEvent(IEventContext eventContext)
        {
            _logger.LogTrace("Event handled by {name}. It will do nothing.", nameof(EmptyEventHandler));
            return Task.FromResult(eventContext.State);
        }
    }
}