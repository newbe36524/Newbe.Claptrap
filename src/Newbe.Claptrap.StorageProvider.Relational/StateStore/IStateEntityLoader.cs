using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public interface IStateEntityLoader<T>
        where T : class, IStateEntity
    {
        Task<T?> GetStateSnapshotAsync();
    }
}