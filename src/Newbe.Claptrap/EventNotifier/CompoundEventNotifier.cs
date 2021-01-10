using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.EventNotifier
{
    public class CompoundEventNotifier : IEventNotifier
    {
        public delegate CompoundEventNotifier Factory(IClaptrapIdentity identity);

        private readonly ILogger<CompoundEventNotifier> _logger;
        private readonly IEventNotifierHandler[] _handlers;

        public CompoundEventNotifier(
            IClaptrapIdentity identity,
            ILogger<CompoundEventNotifier> logger,
            IEnumerable<IEventNotifierHandler> handlers)
        {
            _logger = logger;
            _handlers = handlers.ToArray();
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task Notify(IEventNotifierContext context)
        {
            if (_handlers.Length == 0)
            {
                return Task.CompletedTask;
            }

            return Task.WhenAll(_handlers.Select(x => x.Notify(context).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logger.LogError(t.Exception, "failed to notify event, {@context}", context);
                }
                else
                {
                    _logger.LogTrace("success to notify event, {@context}", context);
                }
            })));
        }
    }
}