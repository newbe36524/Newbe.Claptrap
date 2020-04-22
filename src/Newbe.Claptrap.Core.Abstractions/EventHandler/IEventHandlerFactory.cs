using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventHandler
{
    public interface IEventHandlerFactory
    {
        /// <summary>
        /// create event handler from event context
        /// </summary>
        /// <param name="eventContext"></param>
        /// <exception cref="EventHandlerNotFoundException">thrown if there is no handler found</exception>
        /// <returns></returns>
        IEventHandler Create(IEventContext eventContext);
    }
}