using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE08MinionGrainNoneEventMethodPart
{
    public class GE08CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public MinionMetadata MinionMetadata { get; set; }
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
    }
}