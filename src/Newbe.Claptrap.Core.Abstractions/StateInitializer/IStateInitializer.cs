using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StateInitializer
{
    public interface IStateInitializer
    {
        IActorIdentity ActorIdentity { get; }
        Task<IState> InitializeAsync();
    }
}