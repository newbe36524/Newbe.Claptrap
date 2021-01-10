using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly IOptions<ClaptrapServerOptions> _options;
        private readonly ILogger<ConnectionManager> _logger;
        private readonly Lazy<IConnection> _connectionFactory;
        private IConnection? _connection;

        public ConnectionManager(
            IOptions<ClaptrapServerOptions> options,
            ILogger<ConnectionManager> logger)
        {
            _options = options;
            _logger = logger;
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
            try
            {
                _logger.LogTrace("try to connect rabbit mq");
                var connection = connectionFactory.CreateConnection();
                _connection = connection;
                return connection;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "failed to connect rabbit mq");
                throw;
            }
        }

        public IConnection CreateConnection()
        {
            return _connectionFactory.Value;
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}