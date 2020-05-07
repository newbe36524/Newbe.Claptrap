using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Orleans.Concurrency;

namespace Newbe.Claptrap.Preview.Orleans
{
    public interface IClaptrapMinionGrain : IClaptrapGrain
    {
        Task MasterEventReceivedAsync(IEvent @event);

        [OneWay]
        Task WakeAsync();
    }
}