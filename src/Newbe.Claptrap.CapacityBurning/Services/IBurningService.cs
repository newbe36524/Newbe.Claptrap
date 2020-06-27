using System.Threading.Tasks;

namespace Newbe.Claptrap.CapacityBurning.Services
{
    public interface IBurningService
    {
        Task PrepareAsync()
        {
            return Task.CompletedTask;
        }

        Task CleanAsync()
        {
            return Task.CompletedTask;
        }

        Task StartAsync();
    }
}