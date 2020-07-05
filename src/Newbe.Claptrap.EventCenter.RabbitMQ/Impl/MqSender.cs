using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public class MqSender : IMQSender, IDisposable
    {
        public delegate MqSender Factory(string exchange,
            IReadOnlyDictionary<string, string> topics,
            IConnection connection);

        private readonly string _exchange;
        private readonly IReadOnlyDictionary<string, string> _topics;
        private readonly IConnection _connection;
        private readonly IEventStringSerializer _eventStringSerializer;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IOptions<ClaptrapServerOptions> _options;
        private readonly IBatchOperator<IEvent> _batchOperator;

        public MqSender(
            string exchange,
            IReadOnlyDictionary<string, string> topics,
            IConnection connection,
            IEventStringSerializer eventStringSerializer,
            IMessageSerializer messageSerializer,
            BatchOperator<IEvent>.Factory batchOperatorFactory,
            IBatchOperatorContainer batchOperatorContainer,
            IOptions<ClaptrapServerOptions> options)
        {
            _exchange = exchange;
            _topics = topics;
            _connection = connection;
            _eventStringSerializer = eventStringSerializer;
            _messageSerializer = messageSerializer;
            _options = options;
            var operatorKey = new BatchOperatorKey()
                .With(nameof(MqSender))
                .With(exchange);

            _batchOperator = (IBatchOperator<IEvent>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<IEvent>
                    {
                        BufferTime = TimeSpan.FromMilliseconds(10),
                        BufferCount = 100,
                        DoManyFunc = (events, cacheData) => SendMany(events)
                    }));
        }

        private Task SendMany(IEnumerable<IEvent> entities)
        {
            return Task.Run(SendManyCode);

            void SendManyCode()
            {
                using var model = _connection.CreateModel();

                var batch = model.CreateBasicPublishBatch();
                foreach (var @event in entities)
                {
                    var eventString = _eventStringSerializer.Serialize(@event);
                    var bytes = _messageSerializer.Serialize(eventString);
                    var basicProperties = model.CreateBasicProperties();
                    basicProperties.ContentType = "application/json";

                    var (encoding, body) =
                        CompressData(bytes, _options.Value?.RabbitMQ?.CompressType ?? CompressType.None);
                    basicProperties.ContentEncoding = encoding;
                    batch.Add(_exchange, _topics[@event.EventTypeCode], true, basicProperties, body);
                }

                batch.Publish();
            }
        }

        private static (string? encoding, byte[] data) CompressData(byte[] source, CompressType type)
        {
            if (type == CompressType.None)
            {
                return ("utf8", source);
            }

            using var input = new MemoryStream();
            input.Write(source, 0, source.Length);
            input.Seek(0, SeekOrigin.Begin);
            switch (type)
            {
                case CompressType.GZip:
                    return (GzipStreamHelper.ContentEncoding, GzipStreamHelper.Compress(input));
                case CompressType.Deflate:
                    return (DeflateStreamHelper.ContentEncoding, DeflateStreamHelper.Compress(input));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            throw new ArgumentOutOfRangeException(nameof(type), type, "unsupported compress type");
        }

        public Task SendTopicAsync(IEvent @event)
        {
            return _batchOperator.CreateTask(@event);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}