using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using static Newbe.Claptrap.LK.L0003EmptyEventHandledNotifier;

namespace Newbe.Claptrap.Notifiers
{
    [ExcludeFromCodeCoverage]
    public class EmptyEventNotifier : IEventNotifier
    {
        public delegate EmptyEventNotifier Factory(IClaptrapIdentity identity);

        private readonly ILogger<EmptyEventNotifier> _logger;
        private readonly IL _l;

        public EmptyEventNotifier(
            IClaptrapIdentity identity,
            ILogger<EmptyEventNotifier> logger,
            IL l)
        {
            _logger = logger;
            _l = l;
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task Notify(IEventNotifierContext context)
        {
            _logger.LogTrace(_l[L001EventHandled],
                context.CurrentState.Identity,
                context.Event.Version,
                context.Event.EventTypeCode);
            return Task.CompletedTask;
        }
    }
}