using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    [Minion("AccountDuplicate", "Account", typeof(AccountDuplicateStateData))]
    public interface IAccountDuplicate
        : IMinionGrain
    {
        [MinionEvent(nameof(BalanceChangeEventData))]
        Task HandleBalance(IEvent @event);

        Task<decimal> GetBalance();
    }
}