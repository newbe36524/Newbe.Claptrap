using System.Threading.Tasks;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    public class UnitEventHandler :
        NormalEventHandler<AccountInfo, UnitEvent.UnitEventData>
    {
        public override ValueTask HandleEvent(AccountInfo stateData, UnitEvent.UnitEventData eventData,
            IEventContext eventContext)
        {
            return new ValueTask();
        }
    }
}