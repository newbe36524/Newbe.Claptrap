using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FASTER.core;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.Faster.EventStore
{
    public class FasterEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly FasterLog _log;

        public FasterEventEntitySaver()
        {
            var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "testData.mydata");
            var device = Devices.CreateLogDevice(filename);
            _log = new FasterLog(new FasterLogSettings
            {
                LogDevice = device,
            });
        }

        public Task SaveAsync(EventEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task SaveManyAsync(IEnumerable<EventEntity> entities)
        {
            var source = new CancellationTokenSource();

            foreach (var entity in entities)
            {
                var enqueueAsync = _log.EnqueueAsync(Encoding.UTF8.GetBytes(entity.ToString()), source.Token);
                if (!enqueueAsync.IsCompleted)
                {
                    await enqueueAsync;
                }
            }

            var commitAsync = _log.CommitAsync(source.Token);
            if (!commitAsync.IsCompleted)
            {
                await commitAsync;
            }
        }
    }
}