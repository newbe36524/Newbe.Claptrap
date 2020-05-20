using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public interface IEventSaverProvider
    {
        Task SaveOneAsync(IEvent @event);
        Task SaveManyAsync(IEnumerable<IEvent> events);
    }
}