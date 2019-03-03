using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StateInitializer
{
    public interface IStateInitializerFactory
    {
        IStateInitializer Create(IActorIdentity actorIdentity);
    }
}