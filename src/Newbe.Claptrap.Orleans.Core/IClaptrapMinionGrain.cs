using System.Threading.Tasks;

namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapMinionGrain : IClaptrapGrain
    {
        Task MasterEventReceivedAsync(IEvent @event);

        Task WakeAsync();
    }
}