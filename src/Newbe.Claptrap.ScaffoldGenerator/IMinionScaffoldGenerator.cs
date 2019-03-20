using System.Threading.Tasks;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IMinionScaffoldGenerator
    {
        Task Generate(MinionMetadata minionMetadata);
    }
}