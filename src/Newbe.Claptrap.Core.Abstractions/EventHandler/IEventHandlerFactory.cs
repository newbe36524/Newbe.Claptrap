using Newbe.Claptrap.Context;

namespace Newbe.Claptrap.EventHandler
{
    public interface IEventHandlerFactory
    {
        IEventHandler Create(IEventContext eventContext);
    }
}