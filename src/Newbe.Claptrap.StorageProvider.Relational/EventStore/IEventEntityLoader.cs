using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public interface IEventEntityLoader<T>
        where T : IEventEntity
    {
        Task<IEnumerable<T>> SelectAsync(long startVersion, long endVersion);
    }
}