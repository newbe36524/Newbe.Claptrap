using System.Threading.Tasks;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo.Interfaces.Domain.Account
{
    public interface IAccount : IClaptrapGrain
    {
        Task TransferIn(decimal amount, string uid);

        Task<decimal> GetBalance();
    }
}