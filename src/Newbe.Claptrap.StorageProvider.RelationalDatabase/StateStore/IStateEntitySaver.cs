using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.StateStore
{
    public interface IStateEntitySaver<in T>
    {
        Task SaveAsync(T stateEntity);
    }
}