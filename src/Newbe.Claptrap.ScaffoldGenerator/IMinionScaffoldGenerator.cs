using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IMinionScaffoldGenerator
    {
        Task Generate(MinionScaffoldGeneratorContext context);
    }

    public class MinionScaffoldGeneratorContext
    {
        public MinionMetadata MinionMetadata { get; set; }
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
        public bool IsDomainService { get; set; }
    }
}