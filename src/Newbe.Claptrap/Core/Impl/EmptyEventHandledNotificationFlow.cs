using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Core.Impl
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

        public void OnNewEventHandled(IEventNotifierContext context)
        {
            // nothing
        }
    }
}