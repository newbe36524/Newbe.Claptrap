using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.BalanceChangeEventData;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
namespace Claptrap.N20EventMethods
{
    public class TransferInMethod : ITransferInMethod
    {
        public Task<EventMethodResult<EventData>> Invoke(StateData stateData, decimal amount, string uid)
        {
            throw new NotImplementedException();
        }
    }
}
