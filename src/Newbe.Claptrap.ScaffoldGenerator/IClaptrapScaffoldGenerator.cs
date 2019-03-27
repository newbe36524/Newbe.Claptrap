using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IClaptrapScaffoldGenerator
    {
        Task Generate(ClaptrapScaffoldGeneratorContext context);
    }

    public class ClaptrapScaffoldGeneratorContext
    {
        public ClaptrapMetadata ClaptrapMetadata { get; set; }
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
        public bool IsDomainService { get; set; }
    }
}