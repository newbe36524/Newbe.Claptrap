using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter.RabbitMQ
{
    public interface IMQSender
    {
        Task SendTopicAsync(IEvent @event);
    }
}