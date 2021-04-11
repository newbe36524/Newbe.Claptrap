using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.TestSuit.QuickSetupTools
{
    public class
        AccountBalanceHistoryEventHandler : NormalEventHandler<AccountBalanceHistoryInfo, AccountBalanceChangeEvent>
    {
        public override ValueTask HandleEvent(AccountBalanceHistoryInfo stateData, AccountBalanceChangeEvent eventData,
            IEventContext eventContext)
        {
            var queue = stateData.Balances ?? new Queue<decimal>();
            queue.Enqueue(eventData.Diff);
            while (queue.Count > 10)
            {
                queue.Dequeue();
            }

            return ValueTask.CompletedTask;
        }
    }
}