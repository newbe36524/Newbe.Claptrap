using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;

namespace Newbe.Claptrap.Demo
{
    public class TransferAccountBalanceEventHandler
        : NormalEventHandler<AccountStateData, AccountBalanceChangeEventData>
    {
        public override ValueTask HandleEvent(AccountStateData stateData, AccountBalanceChangeEventData eventData,
            IEventContext eventContext)
        {
            stateData.Balance += eventData.Diff;
            return new ValueTask();
        }
    }
}