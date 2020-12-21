using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageTestWebApi.Services
{
    public interface IInMemoryActorTestService
    {
        Task InitAsync();
        Task<int> RunAsync();
        Task CleanUpAsync();
    }
}