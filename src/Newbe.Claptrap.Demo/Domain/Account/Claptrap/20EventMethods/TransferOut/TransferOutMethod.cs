using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap._20EventMethods.TransferOut
{
    public class TransferOutMethod : ITransferOutMethod
    {
        public Task<EventMethodResult<BalanceChangeEventData, TransferResult>> Invoke(AccountStateData stateData,
            decimal amount, string uid)
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
                BalanceBefore = stateData.Balance
            }, new EventUid(uid));
            return Task.FromResult(result);
        }
    }
}