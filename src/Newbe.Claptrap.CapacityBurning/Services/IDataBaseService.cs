using System.Threading.Tasks;

namespace Newbe.Claptrap.CapacityBurning.Services
{
    public interface IDataBaseService
    {
        Task StartAsync(DatabaseType databaseType, int preparingSleepInSec);
        Task CleanAsync(DatabaseType databaseType);
    }
}