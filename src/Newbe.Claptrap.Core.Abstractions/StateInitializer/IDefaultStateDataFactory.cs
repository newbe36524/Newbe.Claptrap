using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StateInitializer
{
    public interface IDefaultStateDataFactory
    {
        IActorIdentity ActorIdentity { get; }
        Task<IStateData> Create();
    }
}