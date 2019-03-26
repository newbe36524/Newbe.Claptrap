using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.ClaptrapGrainNoneEventMethodPart
{
    public class CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public ClaptrapMetadata ClaptrapMetadata { get; set; }
        
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
    }
}