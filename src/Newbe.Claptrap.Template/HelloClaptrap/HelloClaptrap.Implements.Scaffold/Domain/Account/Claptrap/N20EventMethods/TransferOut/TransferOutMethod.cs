using System;
using System.Threading.Tasks;
using HelloClaptrap.Interfaces.Domain.Account;
using Newbe.Claptrap;
using EventData = HelloClaptrap.Models.Domain.Account.BalanceChangeEventData;
using StateData = HelloClaptrap.Models.Domain.Account.AccountStateData;
namespace HelloClaptrap.Implements.Scaffold.Domain.Account.Claptrap.N20EventMethods.TransferOut
{
    public class TransferOutMethod : ITransferOutMethod
    {
        public Task<EventMethodResult<EventData, TransferResult>> Invoke(StateData stateData, decimal amount, string uid)
        {
            throw new NotImplementedException();
        }
    }
}
