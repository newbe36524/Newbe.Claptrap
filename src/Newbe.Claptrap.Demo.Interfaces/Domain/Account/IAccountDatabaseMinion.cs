using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models.Domain.Account;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    [Minion("Database", "Account")]
    public interface IAccountDatabaseMinion : IMinionGrain
    {
        [MinionEvent(nameof(BalanceChangeEventData))]
        Task SaveBalanceChange(IEvent @event);

        [MinionEvent(nameof(LockEventData))]
        Task Locked(IEvent @event);
    }
}