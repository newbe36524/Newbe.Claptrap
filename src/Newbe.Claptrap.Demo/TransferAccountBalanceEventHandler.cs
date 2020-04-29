using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.Abstractions.Components;

namespace Newbe.Claptrap.Demo
{
    public class TransferAccountBalanceEventHandler
        : ClaptrapEventHandler<AccountStateData, AccountBalanceChangeEventData>
    {
        public override ValueTask HandleEventCore(AccountStateData stateData,
            AccountBalanceChangeEventData eventData, IEventContext context)
        {
            stateData.Balance += eventData.Diff;
            return new ValueTask();
        }
    }
}