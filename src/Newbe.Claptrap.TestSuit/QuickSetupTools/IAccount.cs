using System.Threading.Tasks;

namespace Newbe.Claptrap.TestSuit.QuickSetupTools
{
    [ClaptrapState(typeof(AccountState), Codes.Account)]
    [ClaptrapEvent(typeof(AccountBalanceChangeEvent), Codes.AccountBalanceChangeEvent)]
    [ClaptrapEvent(typeof(UnitEvent.UnitEventData), UnitEvent.TypeCode)]
    public interface IAccount
    {
        Task<decimal> GetBalanceAsync();
        Task ChangeBalanceAsync(decimal diff);
        Task ActivateAsync();
        Task DeactivateAsync();
    }
}