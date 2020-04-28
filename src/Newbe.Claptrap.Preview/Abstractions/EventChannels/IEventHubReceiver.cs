using System.Threading.Tasks;

namespace Newbe.Claptrap.Preview.EventChannels
{
    public interface IEventHubReceiver
    {
        Task StartAsync();
    }
}