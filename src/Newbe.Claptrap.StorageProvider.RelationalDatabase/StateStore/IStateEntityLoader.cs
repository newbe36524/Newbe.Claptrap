using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.StateStore
{
    public interface IStateEntityLoader<T>
    {
        Task<T> GetStateSnapshotAsync();
    }
}