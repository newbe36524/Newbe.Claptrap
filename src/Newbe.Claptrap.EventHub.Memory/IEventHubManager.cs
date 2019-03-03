using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventHub.Memory
{
    public interface IEventHubManager
    {
        Task Publish(IActorIdentity from, IEvent @event);
    }
}