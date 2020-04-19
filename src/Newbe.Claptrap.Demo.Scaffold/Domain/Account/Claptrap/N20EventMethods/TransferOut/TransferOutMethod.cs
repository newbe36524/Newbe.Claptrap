using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.BalanceChangeEventData;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.Account.Claptrap.N20EventMethods.TransferOut
{
    public class TransferOutMethod : ITransferOutMethod
    {
        public Task<EventMethodResult<EventData, TransferResult>> Invoke(StateData stateData, decimal amount, string uid)
        {
            // TODO please add your code here and remove the exception
            throw new NotImplementedException();
        }
    }
}
