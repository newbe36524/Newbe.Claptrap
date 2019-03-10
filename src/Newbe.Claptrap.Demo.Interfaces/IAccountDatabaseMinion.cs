using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models.EventData;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo.Interfaces
{
    [Minion("Database", "Account", typeof(NoneStateData))]
    public interface IAccountDatabaseMinion : IMinionGrain
    {
        [MinionEvent(nameof(BalanceChangeEventData))]
        Task SaveBalanceChange(IEvent @event);

        [MinionEvent(nameof(LockEventData))]
        Task Locked(IEvent @event);
    }
}