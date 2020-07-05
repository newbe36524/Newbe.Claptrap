using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter.RabbitMQ
{
    public interface IMQSenderManager
    {
        Task StartAsync();
        Task CloseAsync();
        IMQSender Get(IEvent evt);
    }
}