using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using EventData = Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance.TransferAccountBalanceFinishedEventData;
using StateData = Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance.TransferAccountBalanceStateData;
namespace Newbe.Claptrap.Demo.Scaffold.DomainService.TransferAccountBalance.Claptrap.N20EventMethods.Transfer
{
    public class TransferMethod : ITransferMethod
    {
        public Task<EventMethodResult<EventData, TransferResult>> Invoke(StateData stateData, string fromId, string toId, decimal balance)
        {
            throw new NotImplementedException();
        }
    }
}
