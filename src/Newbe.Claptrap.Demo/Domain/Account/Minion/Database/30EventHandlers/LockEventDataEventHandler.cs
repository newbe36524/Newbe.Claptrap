using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models.Domain.Account;

namespace Newbe.Claptrap.Demo.Domain.Account.Minion.Database._30EventHandlers
{
    public class LockEventDataEventHandler :
        MinionEventHandlerBase<NoneStateData, LockEventData>
    {
        public override Task HandleEventCore(NoneStateData state, LockEventData @event)
        {
            Console.WriteLine(@event);
            return Task.CompletedTask;
        }
    }
}