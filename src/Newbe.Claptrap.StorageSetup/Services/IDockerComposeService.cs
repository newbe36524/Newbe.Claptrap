using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageSetup
{
    public interface IDockerComposeService
    {
        Task UpAsync(string composeContextDirectory);
        Task DownAsync(string composeContextDirectory);
    }
}