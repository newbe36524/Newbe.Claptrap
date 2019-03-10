using System;
using Autofac;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap.Autofac
{
    public class AutofacStateDataUpdaterFactory : IStateDataUpdaterFactory
    {
        private readonly IComponentContext _componentContext;

        public AutofacStateDataUpdaterFactory(
            IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public IStateDataUpdater Create(IState state, IEvent @event)
        {
            var key = new StateDataUpdaterRegistrationKey(state.Identity.Kind, @event.EventType);
            if (_componentContext.TryResolveKeyed(key,
                    typeof(IStateDataUpdater),
                    out var service)
                && service is IStateDataUpdater updater)
            {
                return updater;
            }

            throw new ArgumentOutOfRangeException(nameof(state.Identity.Kind));
        }
    }
}