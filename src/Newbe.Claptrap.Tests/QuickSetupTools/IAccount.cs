using System.Threading.Tasks;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    [ClaptrapState(typeof(AccountInfo), Codes.Account)]
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