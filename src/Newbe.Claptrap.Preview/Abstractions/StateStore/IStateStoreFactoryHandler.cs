using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.StateStore
{
    public interface IStateStoreFactoryHandler
    {
        IStateStore Create(IActorIdentity identity);
    }
}