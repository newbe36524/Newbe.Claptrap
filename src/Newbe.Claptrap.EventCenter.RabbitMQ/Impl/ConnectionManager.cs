using System;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public class ConnectionManager : IConnectionManager, IDisposable
    {
        private readonly IOptions<ClaptrapServerOptions> _options;
        private readonly Lazy<IConnection> _connectionFactory;
        private IConnection? _connection;

        public ConnectionManager(
            IOptions<ClaptrapServerOptions> options)
        {
            _options = options;
            _connectionFactory = new Lazy<IConnection>(ValueFactory);
        }

        private IConnection ValueFactory()
        {
            var uri = _options.Value?.RabbitMQ?.Uri ?? new Uri("amqp://guest:guest@localhost:5672/%2f");
            var connectionFactory = new ConnectionFactory
            {
                DispatchConsumersAsync = true,
                Uri = uri
            };
            var connection = connectionFactory.CreateConnection();
            _connection = connection;
            return connection;
        }

        public IConnection CreateConnection()
        {
            return _connectionFactory.Value;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}