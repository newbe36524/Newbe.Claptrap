using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.BalanceChangeEventData;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.Account.Claptrap.N20EventMethods.TransferOut
{
    public interface ITransferOutMethod
    {
        Task<EventMethodResult<EventData, TransferResult>> Invoke(StateData stateData, decimal amount, string uid);
    }
}
