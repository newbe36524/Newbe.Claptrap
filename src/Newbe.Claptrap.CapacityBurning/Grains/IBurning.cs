using System.Threading.Tasks;

namespace Newbe.Claptrap.CapacityBurning.Grains
{
    [ClaptrapState(typeof(UnitState.UnitStateData), Codes.Burning)]
    [ClaptrapEvent(typeof(UnitEvent.UnitEventData), Codes.BurningEvent)]
    public interface IBurning
    {
        Task ActivateAsync();
        Task DeactivateAsync();
        Task HandleOneAsync();
    }
}