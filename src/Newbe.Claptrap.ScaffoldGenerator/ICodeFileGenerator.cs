using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface ICodeFileGenerator
    {
        Task<SyntaxTree> Generate();
    }
}