using System.Threading.Tasks;
using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.CapacityBurning
{
    [ClaptrapStateInitialFactoryHandler]
    [ClaptrapEventHandler(typeof(UnitEventHandler), Codes.BurningEvent)]
    public class Burning : NormalClaptrapBox, IBurning
    {
        private readonly IClaptrapIdentity _identity;

        public new delegate Burning Factory(IClaptrapIdentity identity);

        public Burning(IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory) : base(identity,
            claptrapFactory)
        {
            _identity = identity;
        }

        public Task ActivateAsync()
        {
            return Claptrap.ActivateAsync();
        }

        public Task DeactivateAsync()
        {
            return Claptrap.DeactivateAsync();
        }

        public Task HandleOneAsync()
        {
            var evt = new UnitEvent(_identity, Codes.BurningEvent,
                UnitEvent.UnitEventData.Create());
            return Claptrap.HandleEventAsync(evt);
        }
    }
}