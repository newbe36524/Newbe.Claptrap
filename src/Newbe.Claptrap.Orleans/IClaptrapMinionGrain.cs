using System.Threading.Tasks;
using Orleans.Concurrency;

namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapMinionGrain : IClaptrapGrain
    {
        Task MasterEventReceivedAsync(IEvent @event);

        [OneWay]
        Task WakeAsync();
    }
}