using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance.TransferAccountBalanceFinishedEventData;
using StateData = Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance.TransferAccountBalanceStateData;
namespace Claptrap.N20EventMethods
{
    public interface ITransferMethod
    {
        Task<EventMethodResult<EventData, TransferResult>> Invoke(StateData stateData, string fromId, string toId, decimal balance);
    }
}
