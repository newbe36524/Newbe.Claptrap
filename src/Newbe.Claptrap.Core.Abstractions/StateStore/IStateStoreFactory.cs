using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StateStore
{
    public interface IStateStoreFactory
    {
        IStateStore Create(IActorIdentity identity);
    }
}