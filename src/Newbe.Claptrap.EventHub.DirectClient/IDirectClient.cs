using System.Reflection;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.EventHub.DirectClient
{
    public interface IDirectClient
    {
        Task PublishEvent(IMinionGrain grain, IEvent @event, MethodInfo methodInfo = null);
    }
}