using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapEventHandler(typeof(AccountStateData), typeof(AccountBalanceChangeEventData))]
    public class
        TransferAccountBalanceEventHandler : ClaptrapEventHandler<AccountStateData, AccountBalanceChangeEventData>
    {
        public override ValueTask HandleEventCore(AccountStateData stateData,
            AccountBalanceChangeEventData eventData, IEventContext context)
        {
            stateData.Balance += eventData.Diff;
            return new ValueTask();
        }
    }
}