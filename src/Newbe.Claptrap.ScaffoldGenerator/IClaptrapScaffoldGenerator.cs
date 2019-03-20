using System.Threading.Tasks;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IClaptrapScaffoldGenerator
    {
        Task Generate(ClaptrapMetadata claptrapMetadata);
    }
}