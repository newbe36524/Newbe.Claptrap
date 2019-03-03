using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps.EventMethods.AddBalanceImpl
{
    public class AddBalanceMethod : IAddBalanceMethod
    {
        public Task<EventMethodResult<BalanceChangeEventData>> Invoke(AccountStateData stateData,
            decimal amount)
        {
            var result = EventMethodResult.Ok(new BalanceChangeEventData
            {
                Balance = stateData.Balance + amount,
                Amount = amount,
            });
            return Task.FromResult(result);
        }
    }
}