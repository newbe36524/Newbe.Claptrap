using System;
using System.Threading.Tasks;
using Newbe.Claptrap;
using EventData = HelloClaptrap.Models.Domain.Account.BalanceChangeEventData;
using StateData = HelloClaptrap.Models.Domain.Account.AccountStateData;
namespace HelloClaptrap.Implements.Scaffold.Domain.Account.Claptrap.N20EventMethods.TransferIn
{
    public class TransferInMethod : ITransferInMethod
    {
        public Task<EventMethodResult<EventData>> Invoke(StateData stateData, decimal amount, string uid)
        {
            throw new NotImplementedException();
        }
    }
}
