using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models.Domain.Account;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    [Minion("ActorFlow", "Account")]
    public interface IAccountActorFlowMinion : IMinionGrain
    {
        [MinionEvent(nameof(BalanceChangeEventData))]
        Task BalanceChangeNotice(IEvent @event);
    }
}