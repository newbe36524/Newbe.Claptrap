using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance;

namespace Newbe.Claptrap.Demo.DomainService.TransferAccountBalance.Claptrap.N20EventMethods.Transfer
{
    public interface ITransfer
    {
        Task<EventMethodResult<TransferAccountBalanceFinishedEventData>> Invoke(TransferAccountBalanceStateData stateData);
    }
}