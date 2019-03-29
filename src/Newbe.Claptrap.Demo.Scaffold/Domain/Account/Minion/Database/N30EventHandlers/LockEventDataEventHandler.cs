using System;
using System.Threading.Tasks;
using StateData = Newbe.Claptrap.Core.NoneStateData;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.LockEventData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.Account.Minion.Database.N30EventHandlers
{
    public class LockEventDataEventHandler : MinionEventHandlerBase<StateData, EventData>
    {
        public override Task HandleEventCore(StateData stateData, EventData eventData)
        {
            // TODO please add your code here and remove the exception
            throw new NotImplementedException();
        }
    }
}
