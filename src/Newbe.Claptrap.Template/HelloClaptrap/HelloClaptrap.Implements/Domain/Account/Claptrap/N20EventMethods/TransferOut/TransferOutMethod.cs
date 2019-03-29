using System;
using System.Threading.Tasks;
using Newbe.Claptrap;
using EventData = HelloClaptrap.Models.Domain.Account.BalanceChangeEventData;
using StateData = HelloClaptrap.Models.Domain.Account.AccountStateData;
namespace HelloClaptrap.Implements.Domain.Account.Claptrap.N20EventMethods.TransferOut
{
    public class TransferOutMethod : ITransferOutMethod
    {
        public Task<EventMethodResult<EventData, bool>> Invoke(StateData stateData, decimal amount)
        {
            // TODO please add your code here and remove the exception
            throw new NotImplementedException();
        }
    }
}
