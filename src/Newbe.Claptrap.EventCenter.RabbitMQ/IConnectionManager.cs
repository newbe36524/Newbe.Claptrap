using RabbitMQ.Client;

namespace Newbe.Claptrap.EventCenter.RabbitMQ
{
    public interface IConnectionManager
    {
        IConnection CreateConnection();
    }
}