using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public interface ISharedTableEventSaverProvider
    {
        Task SaveAsync(SharedTableEventEntity entity);
    }
}