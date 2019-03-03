using System.Collections.Generic;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap.Autofac
{
    public interface IMinionEventHandlerFactory
    {
        IEnumerable<IEventHandler> Create(IEventContext eventContext);
    }
}