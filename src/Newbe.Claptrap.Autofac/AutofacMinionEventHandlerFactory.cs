using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Metadata;
using Newbe.Claptrap.Autofac.Logging;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap.Autofac
{
    public class AutofacMinionEventHandlerFactory
        : IMinionEventHandlerFactory
    {
        private readonly IComponentContext _componentContext;
        private static readonly ILog Logger = LogProvider.For<AutofacMinionEventHandlerFactory>();

        public AutofacMinionEventHandlerFactory(
            IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public IEnumerable<IEventHandler> Create(IEventContext eventContext)
        {
            if (_componentContext.TryResolve<IEnumerable<Meta<Lazy<IEventHandler>>>>(out var handlers))
            {
                var re = handlers.Where(x =>
                {
                    var minionKind = (IMinionKind) x.Metadata[Constants.MinionEventHandlerMetadataKeys.MinionKind];
                    return eventContext.ActorContext.Identity.Kind.Equals(minionKind)
                           && (string) x.Metadata[Constants.MinionEventHandlerMetadataKeys.EventType] ==
                           eventContext.Event.EventType;
                }).Select(x => x.Value.Value);
                return re;
            }

            Logger.Warn(
                "there is no IEventHandler for Minion ActorKind : {0} , EventType : {1}. Maybe missing registration. please check you configuration.",
                eventContext.ActorContext.Identity.Kind,
                eventContext.Event.EventType);

            return Enumerable.Empty<IEventHandler>();
        }
    }
}