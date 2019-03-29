using System;
using System.Threading.Tasks;
using Newbe.Claptrap;
using EventData = HelloClaptrap.Models.DomainService.TransferAccountBalance.TransferAccountBalanceFinishedEventData;
using StateData = HelloClaptrap.Models.DomainService.TransferAccountBalance.TransferAccountBalanceStateData;
namespace HelloClaptrap.Implements.Scaffold.DomainService.TransferAccountBalance.Claptrap.N20EventMethods.Transfer
{
    public class TransferMethod : ITransferMethod
    {
        public Task<EventMethodResult<EventData, bool>> Invoke(StateData stateData, string fromId, string toId, decimal balance)
        {
            // TODO please add your code here and remove the exception
            throw new NotImplementedException();
        }
    }
}
