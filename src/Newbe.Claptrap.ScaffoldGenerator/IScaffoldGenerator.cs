using System.Threading.Tasks;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IScaffoldGenerator
    {
        ScaffoldGenerateContext Context { get; set; }
        Task Generate();
    }
}