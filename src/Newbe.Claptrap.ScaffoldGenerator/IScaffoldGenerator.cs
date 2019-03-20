using System.Threading.Tasks;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IScaffoldGenerator
    {
        Task Generate(ScaffoldGenerateContext context);
    }
}