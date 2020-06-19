using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap
{
    [ExcludeFromCodeCoverage]
    // ReSharper disable once ClassNeverInstantiated.Global
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