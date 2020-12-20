using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageTestWebApi.Services
{
    public interface ITestService
    {
        Task InitAsync();
        Task<int> RunAsync();
        Task CleanUpAsync();
    }
}