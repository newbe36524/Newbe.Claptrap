using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE02ClaptrapGrainNoneEventMethodPart
{
    public class GE02CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public ClaptrapMetadata ClaptrapMetadata { get; set; }
        
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
    }
}