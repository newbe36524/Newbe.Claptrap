using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap.EventMethods.TransferOut
{
    public interface ITransferOutMethod
    {
        Task<EventMethodResult<BalanceChangeEventData, TransferResult>> Invoke(AccountStateData stateData,
            decimal amount, string uid);
    }
}