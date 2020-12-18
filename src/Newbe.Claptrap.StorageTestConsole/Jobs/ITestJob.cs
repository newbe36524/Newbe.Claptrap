using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageTestConsole
{
    public interface ITestJob
    {
        Task RunAsync();
    }
}