using Newbe.Claptrap.Context;

namespace Newbe.Claptrap.Autofac
{
    public interface IEventLifetimeScope
    {
        IEventContext EventContext { get; }
    }
}