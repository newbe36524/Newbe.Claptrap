using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public interface IStateEntitySaver<in T>
    {
        Task SaveAsync(T stateEntity);
    }
}