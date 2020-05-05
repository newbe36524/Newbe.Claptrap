using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Impl.Localization;
using LK = Newbe.Claptrap.Preview.Impl.Localization.LK.L0003EmptyEventHandledNotifier;

namespace Newbe.Claptrap.Preview.Impl
{
    public class EmptyEventHandledNotifier : IEventHandledNotifier
    {
        public delegate EmptyEventHandledNotifier Factory(IClaptrapIdentity identity);

        private readonly ILogger<EmptyEventHandledNotifier> _logger;
        private readonly IL _l;

        public EmptyEventHandledNotifier(
            IClaptrapIdentity identity,
            ILogger<EmptyEventHandledNotifier> logger,
            IL l)
        {
            _logger = logger;
            _l = l;
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task Notify(IEventHandledNotifierContext context)
        {
            _logger.LogTrace(_l[LK.L001EventHandled],
                context.CurrentState.Identity,
                context.Event.Version,
                context.Event.EventTypeCode);
            return Task.CompletedTask;
        }
    }
}