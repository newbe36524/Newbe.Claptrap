using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Impl.AccountImpl.Minions.Database
{
    public class AccountMinionBalanceChangeEventHandler2 :
        MinionEventHandlerBase<NoneStateData, BalanceChangeEventData>
    {
        public override Task HandleEventCore(NoneStateData state, BalanceChangeEventData @event)
        {
            Console.WriteLine($"minion actor DataBase 2 receive event: {@event}");
            return Task.CompletedTask;
        }
    }
}