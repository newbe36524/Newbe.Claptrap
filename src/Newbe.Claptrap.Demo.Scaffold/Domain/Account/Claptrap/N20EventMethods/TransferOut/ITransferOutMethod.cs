using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.BalanceChangeEventData;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
namespace Claptrap.N20EventMethods
{
    public interface ITransferOutMethod
    {
        Task<EventMethodResult<EventData, TransferResult>> Invoke(StateData stateData, decimal amount, string uid);
    }
}
