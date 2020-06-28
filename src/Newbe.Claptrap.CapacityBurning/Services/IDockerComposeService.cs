using System.Threading.Tasks;

namespace Newbe.Claptrap.CapacityBurning.Services
{
    public interface IDockerComposeService
    {
        Task UpAsync(string composeContextDirectory);
        Task DownAsync(string composeContextDirectory);
    }
}