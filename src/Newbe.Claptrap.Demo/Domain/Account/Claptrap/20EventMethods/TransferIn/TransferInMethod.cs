using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap._20EventMethods.TransferIn
{
    public class TransferInMethod : ITransferInMethod
    {
        public Task<EventMethodResult<BalanceChangeEventData>> Invoke(AccountStateData stateData,
            decimal amount, string uid)
        {
            var result = EventMethodResult.Ok<BalanceChangeEventData>(new BalanceChangeEventData
            {
                Balance = stateData.Balance + amount,
                Amount = amount
            }, new EventUid(uid));
            return Task.FromResult(result);
        }
    }
}