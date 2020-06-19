using System.Threading.Tasks;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    [ClaptrapState(typeof(AccountInfo), Codes.Account)]
    [ClaptrapEvent(typeof(AccountBalanceChangeEvent), Codes.AccountBalanceChangeEvent)]
    public interface IAccount
    {
        Task<decimal> GetBalanceAsync();
        Task ChangeBalanceAsync(decimal diff);
        Task ActivateAsync();
        Task DeactivateAsync();
    }
}