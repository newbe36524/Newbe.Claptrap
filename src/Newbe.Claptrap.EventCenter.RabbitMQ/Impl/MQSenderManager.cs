using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.Extensions;
using RabbitMQ.Client;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public class MQSenderManager : IMQSenderManager, IDisposable
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly IConnectionManager _connectionManager;
        private readonly MqSender.Factory _mqSenderFactory;
        private IReadOnlyDictionary<string, MqSender>? _senders;
        private IConnection? _connection;

        public MQSenderManager(
            IClaptrapDesignStore claptrapDesignStore,
            IConnectionManager connectionManager,
            MqSender.Factory mqSenderFactory)
        {
            _claptrapDesignStore = claptrapDesignStore;
            _connectionManager = connectionManager;
            _mqSenderFactory = mqSenderFactory;
        }

        public Task StartAsync()
        {
            return Task.Run(InitCore);

            void InitCore()
            {
                _connection = _connectionManager.CreateConnection();
                _senders = _claptrapDesignStore
                    .Where(x => !x.IsMinion())
                    .ToDictionary(x => x.ClaptrapTypeCode, x =>
                    {
                        var exchangeName = TopicHelper.GetExchangeName(x.ClaptrapTypeCode);
                        var topics = x.EventHandlerDesigns
                            .ToDictionary(a => a.Key, a => TopicHelper.GetRouteKey(x.ClaptrapTypeCode, a.Key));
                        using var model = _connection.CreateModel();
                        model.ExchangeDeclare(exchangeName, ExchangeType.Topic);
                        var mqSender = _mqSenderFactory.Invoke(exchangeName, topics, _connection);
                        return mqSender;
                    });
            }
        }

        public Task CloseAsync()
        {
            return Task.Run(Dispose);
        }

        public IMQSender Get(IEvent evt)
        {
            Debug.Assert(_senders != null, nameof(_senders) + " != null");
            var sender = _senders[evt.ClaptrapIdentity.TypeCode];
            return sender;
        }

        public void Dispose()
        {
            _connectionManager.Dispose();
        }
    }
}