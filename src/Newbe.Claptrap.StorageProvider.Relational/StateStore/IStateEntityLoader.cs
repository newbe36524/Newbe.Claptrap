using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public interface IStateEntityLoader<T>
    {
        Task<T> GetStateSnapshotAsync();
    }
}