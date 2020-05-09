using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapMinionGrain : IClaptrapGrain
    {
        Task MasterEventReceivedAsync(IEvent @event);

        Task WakeAsync();
    }
}