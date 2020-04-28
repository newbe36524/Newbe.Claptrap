using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.StateStore
{
    public interface IStateStoreFactory
    {
        IStateStore Create(IActorIdentity identity);
    }
}