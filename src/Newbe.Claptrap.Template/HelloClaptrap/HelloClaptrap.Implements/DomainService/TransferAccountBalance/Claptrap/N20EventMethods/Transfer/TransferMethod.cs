using System;
using System.Threading.Tasks;
using HelloClaptrap.Interfaces.Domain.Account;
using Newbe.Claptrap;
using EventData = HelloClaptrap.Models.DomainService.TransferAccountBalance.TransferAccountBalanceFinishedEventData;
using StateData = HelloClaptrap.Models.DomainService.TransferAccountBalance.TransferAccountBalanceStateData;
namespace HelloClaptrap.Implements.DomainService.TransferAccountBalance.Claptrap.N20EventMethods.Transfer
{
    public class TransferMethod : ITransferMethod
    {
        public Task<EventMethodResult<EventData, TransferResult>> Invoke(StateData stateData, string fromId, string toId, decimal balance)
        {
            throw new NotImplementedException();
        }
    }
}
