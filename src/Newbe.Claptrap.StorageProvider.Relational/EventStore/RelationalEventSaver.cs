using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class RelationalEventSaver<T> : IRelationalEventSaver
        where T : IEventEntity
    {
        private readonly IEventEntityMapper<T> _mapper;
        private readonly IEventEntitySaver<T> _saver;
        private readonly ILogger<RelationalEventSaver<T>> _logger;

        public RelationalEventSaver(
            IClaptrapIdentity identity,
            ILogger<RelationalEventSaver<T>> logger,
            IEventEntityMapper<T> mapper,
            IEventEntitySaver<T> saver)
        {
            _logger = logger;
            Identity = identity;
            _mapper = mapper;
            _saver = saver;
        }

        public IClaptrapIdentity Identity { get; }

        public async Task SaveEventAsync(IEvent @event)
        {
            try
            {
                await SaveEventAsyncCore();
            }
            catch (Exception e)
            {
                throw new EventSavingException(e, @event);
            }

            async Task SaveEventAsyncCore()
            {
                var entity = _mapper.Map(@event);
                _logger.LogDebug("start to save event to store {@eventEntity}", entity);
                await _saver.SaveAsync(entity);
            }
        }
    }
}