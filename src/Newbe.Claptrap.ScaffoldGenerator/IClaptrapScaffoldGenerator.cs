using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IClaptrapScaffoldGenerator
    {
        Task Generate(ClaptrapMetadata claptrapMetadata, CompilationUnitSyntax compilationUnitSyntax);
    }
}