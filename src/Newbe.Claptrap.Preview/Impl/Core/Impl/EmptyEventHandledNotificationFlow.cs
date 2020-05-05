using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Components;

namespace Newbe.Claptrap.Preview.Impl
{
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
            // nothing
        }

        public void Deactivate()
        {
            // nothing
        }

        public void OnNewEventHandled(IEventHandledNotifierContext context)
        {
            // nothing
        }
    }
}