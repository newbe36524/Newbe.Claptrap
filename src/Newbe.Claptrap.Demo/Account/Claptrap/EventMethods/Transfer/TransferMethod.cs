using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Account.Claptrap.EventMethods.Transfer
{
    public class TransferMethod : ITransferMethod
    {
        public Task<EventMethodResult<BalanceChangeEventData, TransferResult>> Invoke(
            AccountStateData stateData,
            decimal amount)
        {
            if (stateData.Balance < amount)
            {
                return Task.FromResult(EventMethodResult.None<BalanceChangeEventData, TransferResult>(new TransferResult
                {
                    Error = "insufficient funds",
                    BalanceBefore = stateData.Balance,
                    BalanceNow = stateData.Balance,
                }));
            }

            var eventData = new BalanceChangeEventData
            {
                Amount = -amount,
                Balance = stateData.Balance - amount,
            };
            var result = EventMethodResult.Ok(eventData, new TransferResult
            {
                BalanceNow = eventData.Balance,
                BalanceBefore = stateData.Balance,
            });
            return Task.FromResult(result);
        }
    }
}