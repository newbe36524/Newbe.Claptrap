using System.Threading.Tasks;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    public interface IAccount : IGrainWithStringKey
    {
        Task TransferIn(decimal amount, string uid);

        Task<decimal> GetBalance();
    }
}