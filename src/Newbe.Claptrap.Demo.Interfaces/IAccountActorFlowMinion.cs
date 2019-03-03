using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo.Interfaces
{
    [Minion("ActorFlow", "Account", typeof(NoneStateData))]
    public interface IAccountActorFlowMinion : IMinionGrain
    {
    }
}