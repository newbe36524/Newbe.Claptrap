using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StateInitializer
{
    public interface IStateDataFactory
    {
        IActorIdentity ActorIdentity { get; }
        Task<IStateData> CreateInitialState();
    }
}