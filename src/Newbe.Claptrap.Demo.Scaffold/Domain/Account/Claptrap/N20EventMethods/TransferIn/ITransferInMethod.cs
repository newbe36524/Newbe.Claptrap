using System.Threading.Tasks;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.BalanceChangeEventData;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.Account.Claptrap.N20EventMethods.TransferIn
{
    public interface ITransferInMethod
    {
        Task<EventMethodResult<EventData>> Invoke(StateData stateData, decimal amount, string uid);
    }
}
