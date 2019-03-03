using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models.EventData;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo.Interfaces
{
    [Minion("Database", "Account", typeof(NoneStateData))]
    [MinionEvent(nameof(BalanceChangeEventData))]
    [MinionEvent(nameof(LockEventData))]
    public interface IAccountDatabaseMinion : IMinionGrain
    {
    }
}