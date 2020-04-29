using Newbe.Claptrap.Preview.Abstractions.Exceptions;

namespace Newbe.Claptrap.Preview.Abstractions.Components
{
    public interface IEventHandlerFactory : IClaptrapComponent
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