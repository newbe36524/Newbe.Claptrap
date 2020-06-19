using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap
{
    public class CompoundEventNotifierFactory : IClaptrapComponentFactory<IEventNotifier>
    {
        private readonly CompoundEventNotifier.Factory _factory;
        private readonly ILifetimeScope _lifetimeScope;

        public CompoundEventNotifierFactory(
            CompoundEventNotifier.Factory factory,
            ILifetimeScope lifetimeScope)
        {
            _factory = factory;
            _lifetimeScope = lifetimeScope;
        }

        public IEventNotifier Create(IClaptrapIdentity claptrapIdentity)
        {
            var compoundEventNotifier = _factory.Invoke(claptrapIdentity);
            return compoundEventNotifier;
        }
    }

    public class CompoundEventNotifier : IEventNotifier
    {
        public delegate CompoundEventNotifier Factory(IClaptrapIdentity identity);

        private readonly ILogger<CompoundEventNotifier> _logger;
        private readonly IEnumerable<IEventNotifier> _eventNotifiers;

        public CompoundEventNotifier(
            IClaptrapIdentity identity,
            ILogger<CompoundEventNotifier> logger,
            IEnumerable<IEventNotifier> eventNotifiers)
        {
            _logger = logger;
            _eventNotifiers = eventNotifiers;
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task Notify(IEventNotifierContext context)
        {
            return Task.WhenAll(_eventNotifiers.Select(x => x.Notify(context).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logger.LogError("failed to notify event, {@context}", context);
                }
            })));
        }
    }
}