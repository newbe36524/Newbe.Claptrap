using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StateStore
{
    public interface IStateStoreFactoryHandler
    {
        IStateStore Create(IActorIdentity identity);
    }
}