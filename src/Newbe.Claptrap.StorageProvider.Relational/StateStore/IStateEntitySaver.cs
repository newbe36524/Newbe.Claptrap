using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public interface IStateEntitySaver<in T>
        where T : class, IStateEntity
    {
        Task SaveAsync(T stateEntity);
    }
}