using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Autofac
{
    public interface IActorLifetimeScope
    {
        IActorIdentity Identity { get; }
    }
}