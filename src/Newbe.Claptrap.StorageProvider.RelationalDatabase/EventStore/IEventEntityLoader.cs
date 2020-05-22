using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore
{
    public interface IEventEntityLoader<T>
        where T : IEventEntity
    {
        Task<IEnumerable<T>> SelectAsync(long startVersion, long endVersion);
    }
}