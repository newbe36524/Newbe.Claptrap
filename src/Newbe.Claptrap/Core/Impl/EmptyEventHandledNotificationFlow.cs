using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Core.Impl
{
    [ExcludeFromCodeCoverage]
    public class EmptyEventHandledNotificationFlow : IEventHandledNotificationFlow
    {
        private readonly ILogger<EmptyEventHandledNotificationFlow> _logger;

        public EmptyEventHandledNotificationFlow(
            ILogger<EmptyEventHandledNotificationFlow> logger)
        {
            _logger = logger;
        }

        public void Activate()
        {
            _logger.LogTrace("Activated");
            // nothing
        }

        public void Deactivate()
        {
            _logger.LogTrace("Deactivated");
            // nothing
        }

        public void OnNewEventHandled(IEventNotifierContext context)
        {
            _logger.LogTrace("Received a context {@context} and do nothing", context);
            // nothing
        }
    }
}