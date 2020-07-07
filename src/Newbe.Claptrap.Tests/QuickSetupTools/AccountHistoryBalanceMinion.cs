using System.Collections.Generic;
using System.Threading.Tasks;
using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    [ClaptrapEventHandler(typeof(AccountBalanceHistoryEventHandler), Codes.AccountBalanceChangeEvent)]
    [ClaptrapEventHandler(typeof(EmptyEventHandler), UnitEvent.TypeCode)]
    public class AccountHistoryBalanceMinion :
        NormalClaptrapBox<AccountBalanceHistoryInfo>, IAccountHistoryBalanceMinion
    {
        public AccountHistoryBalanceMinion(IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory,
            IClaptrapAccessor claptrapAccessor) : base(identity,
            claptrapFactory,
            claptrapAccessor)
        {
        }

        public Task<IEnumerable<decimal>> GetBalanceHistoryAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task ActivateAsync()
        {
            return Claptrap.ActivateAsync();
        }
    }
}