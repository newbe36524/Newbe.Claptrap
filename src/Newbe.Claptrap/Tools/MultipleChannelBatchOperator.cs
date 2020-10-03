using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newbe.Claptrap.MultipleChannelBatchOperators;

namespace Newbe.Claptrap
{
    public class MultipleChannelBatchOperator<T> : IBatchOperator<T>
    {
        private readonly ObjectPool<BatchItem> _itemPool;

        public delegate MultipleChannelBatchOperator<T> Factory(BatchOperatorOptions<T> options);

        private readonly Channel<BatchItem> _channel;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly BatchItemPacker<T> _batchItemPacker;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly TaskConsumer<T> _taskConsumer;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ResultSetter<T> _resultSetter;

        public MultipleChannelBatchOperator(
            BatchOperatorOptions<T> options,
            ObjectPool<BatchItem> itemPool,
            ILogger<MultipleChannelBatchOperator<T>> logger)
        {
            if (options.BufferTime == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.BufferCount == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _itemPool = itemPool;
            _channel = Channel.CreateBounded<BatchItem>(new BoundedChannelOptions(100_000)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
            var itemsChannel = Channel.CreateUnbounded<BatchItems>();
            var resultChannel = Channel.CreateUnbounded<BatchItems>();

            _batchItemPacker = new BatchItemPacker<T>(
                options,
                logger,
                _channel,
                itemsChannel);
            _batchItemPacker.Start();
            _taskConsumer = new TaskConsumer<T>(
                options,
                logger,
                itemsChannel,
                resultChannel);
            _taskConsumer.Start();

            _resultSetter = new ResultSetter<T>(
                logger,
                itemPool,
                resultChannel);
            _resultSetter.Start();
        }

        public async ValueTask CreateTask(T input)
        {
            var item = _itemPool.Get();
            item.Input = input;
            var valueTask = _channel.Writer.WriteAsync(item);
            if (!valueTask.IsCompleted)
            {
                await valueTask;
            }

            var finalValueTask = new ValueTask(item.Vts, item.Vts.Version);
            if (!finalValueTask.IsCompleted)
            {
                await finalValueTask;
            }
        }

        public class BatchItem
        {
            public T Input { get; set; } = default!;
            public ManualResetValueTaskSource<int> Vts { get; set; } = null!;
        }

        public class BatchItems
        {
            public BatchItem[] Items { get; set; } = null!;
            public int ItemCount { get; set; }
            public Exception? Exception { get; set; }
        }
    }
}