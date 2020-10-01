using System.Threading.Tasks;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    public class UnitEventHandler :
        NormalEventHandler<AccountState, UnitEvent.UnitEventData>
    {
        public override ValueTask HandleEvent(AccountState stateData, UnitEvent.UnitEventData eventData,
            IEventContext eventContext)
        {
            return new ValueTask();
        }
    }
}