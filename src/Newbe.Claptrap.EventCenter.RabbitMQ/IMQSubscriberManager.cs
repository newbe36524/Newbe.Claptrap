using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter.RabbitMQ
{
    public interface IMQSubscriberManager
    {
        Task StartAsync();
        Task CloseAsync();
    }
}