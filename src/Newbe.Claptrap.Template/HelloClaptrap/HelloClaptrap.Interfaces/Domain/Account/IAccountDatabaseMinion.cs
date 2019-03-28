using System.Threading.Tasks;
using HelloClaptrap.Models.Domain.Account;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;

namespace HelloClaptrap.Interfaces.Domain.Account
{
    [Minion("Database", "Account")]
    public interface IAccountDatabaseMinion : IMinionGrain
    {
        [MinionEvent(nameof(BalanceChangeEventData))]
        Task SaveBalanceChange(IEvent @event);
    }
}