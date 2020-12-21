using System.Threading.Tasks;

namespace Newbe.Claptrap.Demo.Server.Services
{
    public interface IOrleansActorTestService
    {
        Task InitAsync();
        Task<int> RunAsync();
        Task CleanUpAsync();
    }
}