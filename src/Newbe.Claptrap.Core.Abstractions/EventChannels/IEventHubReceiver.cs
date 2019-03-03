using System.Threading.Tasks;

namespace Newbe.Claptrap.EventChannels
{
    public interface IEventHubReceiver
    {
        Task StartAsync();
    }
}