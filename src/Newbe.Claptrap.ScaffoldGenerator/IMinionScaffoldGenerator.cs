using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IMinionScaffoldGenerator
    {
        Task Generate(MinionMetadata minionMetadata, CompilationUnitSyntax compilationUnitSyntax);
    }
}