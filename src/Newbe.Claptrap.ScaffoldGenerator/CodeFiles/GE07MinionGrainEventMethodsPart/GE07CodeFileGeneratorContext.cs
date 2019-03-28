using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE07MinionGrainEventMethodsPart
{
    public class GE07CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public MinionMetadata MinionMetadata { get; set; }
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
    }
}