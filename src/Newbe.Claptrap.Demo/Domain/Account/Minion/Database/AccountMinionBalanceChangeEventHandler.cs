using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Domain.Account.Minion.Database
{
    public class AccountMinionBalanceChangeEventHandler :
        MinionEventHandlerBase<NoneStateData, BalanceChangeEventData>
    {
        public override Task HandleEventCore(NoneStateData state, BalanceChangeEventData @event)
        {
//            Console.WriteLine($"minion actor DataBase 1 receive event: {@event}");
            return Task.CompletedTask;
        }
    }
}