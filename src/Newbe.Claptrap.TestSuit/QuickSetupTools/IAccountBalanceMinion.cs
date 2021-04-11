using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.TestSuit.QuickSetupTools
{
    [ClaptrapMinion(Codes.Account)]
    [ClaptrapState(typeof(AccountState), Codes.AccountBalanceMinion)]
    public interface IAccountBalanceMinion
    {
        Task<decimal> GetBalanceAsync();
        Task ActivateAsync();
    }

    [ClaptrapMinion(Codes.Account)]
    [ClaptrapState(typeof(AccountBalanceHistoryInfo), Codes.AccountBalanceHistoryMinion)]
    public interface IAccountHistoryBalanceMinion
    {
        Task<IEnumerable<decimal>> GetBalanceHistoryAsync();
    }

    public class AccountBalanceHistoryInfo : IStateData
    {
        public Queue<decimal> Balances { get; set; }
    }
}