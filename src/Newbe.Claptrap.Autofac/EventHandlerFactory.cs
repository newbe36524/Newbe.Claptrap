using System;
using System.Diagnostics.Tracing;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.EventHandlers;

namespace Newbe.Claptrap.Autofac
{
    public class EventHandlerFactory : IEventHandlerFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public delegate ClaptrapStateEventHandler EventSourcingEventHandlerFactory();

        public delegate MinionEventHandler MinionEventHandlerFactory();

        public delegate StateRestoreEventHandler StateRestoreEventHandlerFactory(IEventHandler eventHandler);

        public delegate ClaptrapEventPublishEventHandler ClaptrapEventHubEventHandlerFactory();

        public EventHandlerFactory(
            ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IEventHandler Create(IEventContext eventContext)
        {
            var eventScope = _lifetimeScope.BeginLifetimeScope(Constants.EventLifetimeScope);

            var eventLifetimeScope = eventScope.Resolve<AutofacEventLifetimeScope>();
            eventLifetimeScope.EventContext = eventContext;

            IEventHandler inner;
            switch (eventContext.ActorContext.Identity.Kind.ActorType)
            {
                case ActorType.Claptrap:
                    var eventSourcingHandler = eventScope.Resolve<EventSourcingEventHandlerFactory>().Invoke();
                    var stateRestoreHandler = eventScope.Resolve<StateRestoreEventHandlerFactory>()
                        .Invoke(eventSourcingHandler);
                    var claptrapEventHubEventHandler =
                        eventScope.Resolve<ClaptrapEventHubEventHandlerFactory>().Invoke();
                    var eventHandler = new OrderedEventHandler(new IEventHandler[]
                        {stateRestoreHandler, claptrapEventHubEventHandler});
                    inner = eventHandler;
                    break;
                case ActorType.Minion:
                    if (!(eventContext.ActorContext.Identity.Kind is IMinionKind minionKind))
                    {
                        throw new ArgumentOutOfRangeException(nameof(eventContext.ActorContext.Identity.Kind));
                    }

                    var stateHandler = eventScope.Resolve<MinionEventHandlerFactory>().Invoke();
                    var minionEventHandlerFactory = eventScope.Resolve<IMinionEventHandlerFactory>();
                    var eventHandlers = minionEventHandlerFactory.Create(eventContext);
                    var multipleAsyncEventHandler = new MultipleAsyncEventHandler(eventHandlers);
                    var orderedEventHandler = new OrderedEventHandler(new IEventHandler[]
                        {stateHandler, multipleAsyncEventHandler});
                    inner = eventScope.Resolve<StateRestoreEventHandlerFactory>().Invoke(orderedEventHandler);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var re = new AutofacEventHandler(inner, eventScope);
            return re;
        }

        internal class AutofacEventHandler : IEventHandler
        {
            private readonly ILifetimeScope _lifetimeScope;
            private readonly IEventHandler _eventHandler;

            public AutofacEventHandler(
                IEventHandler eventHandler,
                ILifetimeScope lifetimeScope)
            {
                _lifetimeScope = lifetimeScope;
                _eventHandler = eventHandler;
            }

            public ValueTask DisposeAsync()
            {
                _lifetimeScope.Dispose();
                return _eventHandler.DisposeAsync();
            }

            public Task HandleEvent(IEventContext eventContext)
            {
                return _eventHandler.HandleEvent(eventContext);
            }
        }
    }
}