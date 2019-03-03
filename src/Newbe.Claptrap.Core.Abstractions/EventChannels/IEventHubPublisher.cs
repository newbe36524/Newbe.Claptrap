using System.Threading.Tasks;

namespace Newbe.Claptrap.EventChannels
{
    public interface IEventHubPublisher
    {
        Task StartAsync();
    }
}