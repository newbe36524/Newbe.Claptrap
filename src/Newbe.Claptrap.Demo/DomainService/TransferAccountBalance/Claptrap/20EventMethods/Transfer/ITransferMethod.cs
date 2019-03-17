using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.DomainService.TransferAccountBalance;
using Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance;

namespace Newbe.Claptrap.Demo.DomainService.TransferAccountBalance.Claptrap._20EventMethods.Transfer
{
    public interface ITransfer
    {
        Task<EventMethodResult<TransferAccountBalanceFinishedEventData>> Invoke(TransferAccountBalanceStateData stateData);
    }
}