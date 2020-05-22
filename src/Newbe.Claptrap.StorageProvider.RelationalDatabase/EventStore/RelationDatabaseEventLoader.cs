using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore
{
    public class RelationDatabaseEventLoader<T> : IEventLoader
        where T : IEventEntity
    {
        private readonly IEventEntityMapper<T> _mapper;
        private readonly IEventEntityLoader<T> _loader;
        private readonly ILogger _logger;

        public RelationDatabaseEventLoader(
            IClaptrapIdentity identity,
            ILogger logger,
            IEventEntityMapper<T> mapper,
            IEventEntityLoader<T> loader)
        {
            _logger = logger;
            Identity = identity;
            _mapper = mapper;
            _loader = loader;
        }

        public IClaptrapIdentity Identity { get; }

        public async Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            var entities = await _loader.SelectAsync(startVersion, endVersion);
            var re = entities.Select(x => _mapper.Map(x, Identity)).ToArray();
            _logger.LogDebug("found {count} events that version in range [{startVersion}, {endVersion}).",
                re.Length,
                startVersion,
                endVersion);
            return re;
        }
    }
}