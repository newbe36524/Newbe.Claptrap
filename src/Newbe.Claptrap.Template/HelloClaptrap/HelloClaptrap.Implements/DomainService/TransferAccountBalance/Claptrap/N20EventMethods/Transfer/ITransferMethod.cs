using System.Threading.Tasks;
using Newbe.Claptrap;
using EventData = HelloClaptrap.Models.DomainService.TransferAccountBalance.TransferAccountBalanceFinishedEventData;
using StateData = HelloClaptrap.Models.DomainService.TransferAccountBalance.TransferAccountBalanceStateData;
namespace HelloClaptrap.Implements.DomainService.TransferAccountBalance.Claptrap.N20EventMethods.Transfer
{
    public interface ITransferMethod
    {
        Task<EventMethodResult<EventData, bool>> Invoke(StateData stateData, string fromId, string toId, decimal balance);
    }
}
