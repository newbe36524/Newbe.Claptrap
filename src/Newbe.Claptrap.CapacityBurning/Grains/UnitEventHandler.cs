using System.Threading.Tasks;

namespace Newbe.Claptrap.CapacityBurning.Grains
{
    public class UnitEventHandler :
        NormalEventHandler<UnitState.UnitStateData, UnitEvent.UnitEventData>
    {
        public override ValueTask HandleEvent(UnitState.UnitStateData stateData, UnitEvent.UnitEventData eventData,
            IEventContext eventContext)
        {
            stateData.Item1 = eventData.Item1;
            stateData.Item2 = eventData.Item2;
            stateData.Item3 = eventData.Item3;
            stateData.Item4 = eventData.Item4;
            stateData.Item5 = eventData.Item5;
            return new ValueTask();
        }
    }
}